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
                var samples = new float[reader.Length / (reader.WaveFormat.BitsPerSample / 8)];
                var span = new Span<byte>(new byte[16 * 1024]);
                while ((readed = reader.Read(span)) > 0)
                {
                    var subSpan = span.Slice(0, (int)readed);
                }

                sound.Samples = samples;
            }
        });
    }

    public void Add(RecordedSound x)
    {
        _items.Add(x);
        Changed?.Invoke();
    }
}