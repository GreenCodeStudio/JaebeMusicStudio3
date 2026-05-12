using System.Text.Json.Serialization;
using JaebeMusicStudio3.Core.Mixer;
using JaebeMusicStudio3.Core.Utils;
using NAudio.Wave;

namespace JaebeMusicStudio3.Core.Timeline;

public class Timeline
{
    public static Timeline Current = new Timeline();
    [JsonPropertyName("Items")]
    [JsonInclude]
    private List<ITimelineItem> _items = new List<ITimelineItem>();
    [JsonIgnore]
    public IReadOnlyList<ITimelineItem> Items => _items.ToList();
    public double TotalLength => 60; //tmp

    public event Action Changed;

    public void LoadFileAsync(string fileName)
    {
        Task.Run(() =>
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
        });
    }

    public void Add(ITimelineItem x)
    {
        _items.Add(x);
        Changed?.Invoke();
    }
}