using System.Text.Json.Serialization;
using JaebeMusicStudio3.Core.Mixer;
using JaebeMusicStudio3.Core.Utils;
using NAudio.Midi;
using NAudio.Wave;

namespace JaebeMusicStudio3.Core.Timeline;

public class Timeline
{
    public static Timeline Current = new Timeline();

    [JsonPropertyName("Items")] [JsonInclude]
    private List<ITimelineItem> _items = new List<ITimelineItem>();

    [JsonIgnore] public IReadOnlyList<ITimelineItem> Items => _items.ToList();
    public double TotalLength => 60; //tmp

    public event Action Changed;

    public void LoadFileAsync(string fileName)
    {
        Task.Run(() =>
        {
            if (fileName.EndsWith(".midi", StringComparison.InvariantCultureIgnoreCase) ||
                fileName.EndsWith(".mid", StringComparison.InvariantCultureIgnoreCase))
            {
                var file = new MidiFile(fileName);
                foreach (var list in file.Events)
                {
                    var linesPerChannels = new Dictionary<int, NoteLine>();

                    foreach (var e in list)
                    {
                        if (!linesPerChannels.ContainsKey(e.Channel))
                        {
                            var line = new NoteLine();
                            linesPerChannels.Add(e.Channel, line);
                            this.Add(line);
                        }

                        if (e is TextEvent te)
                        {
                            linesPerChannels[e.Channel].Name = te.Text;
                        }

                        if (e is TempoEvent teo)
                        {
                            linesPerChannels[e.Channel].Tempo = teo.Tempo;
                        }

                        if (e is NoteOnEvent neo)
                        {
                            var note = new Note()
                            {
                                Start = (double)neo.AbsoluteTime / file.DeltaTicksPerQuarterNote ,
                                Pitch = 8.1758 * Math.Pow(2, neo.NoteNumber / 12.0),
                                // Volume = (float)neo.Velocity / 127,
                                Length = neo.OffEvent==null?0:((double)neo.NoteLength / file.DeltaTicksPerQuarterNote)
                            };
                            linesPerChannels[e.Channel].Notes.Add(note);
                        }
                    }
                }
            }
            else
            {
                using (NAudio.Wave.AudioFileReader reader = new AudioFileReader(fileName))
                {
                    var sound = new RecordedSound();
                    sound.WaveFormat = reader.WaveFormat;
                    long readed;
                    var span = new Span<byte>(new byte[reader.Length]);
                    reader.Read(span);
                    var samples = BinaryConverter.FromBinary(reader.WaveFormat, span);

                    sound.Samples = samples;
                    Add(sound);
                    AudioMixer.Current.Add(sound);
                }
            }
        });
    }

    public void Add(ITimelineItem x)
    {
        x.LineNumber = _items.Count;
        _items.Add(x);
        Changed?.Invoke();
    }
}