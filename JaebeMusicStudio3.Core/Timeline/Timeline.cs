using JaebeMusicStudio3.Core.Mixer;
using JaebeMusicStudio3.Core.Utils;
using NAudio.Wave;

namespace JaebeMusicStudio3.Core.Timeline;

public class Timeline
{
    private List<RecordedSound> _items = new List<RecordedSound>();
    public IReadOnlyList<RecordedSound> Items => _items.ToList();
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
                var samples=BinaryConverter.FromBinary(reader.WaveFormat, span);

                sound.Samples = samples;
                Add(sound);
                AudioMixer.Current.Add(sound);
            }
        });
    }

    public void Add(RecordedSound x)
    {
        _items.Add(x);
        Changed?.Invoke();
    }
}