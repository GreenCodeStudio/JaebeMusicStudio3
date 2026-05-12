using System.IO.Compression;
using System.Text.Json;
using JaebeMusicStudio3.Core.Mixer;

namespace JaebeMusicStudio3.Core.Serialization;

public class Project
{
    public void SaveAsFile(string path)
    {
        using (var zip = ZipFile.Open(path, ZipArchiveMode.Create))
        {
            var entry = zip.CreateEntry("project.json");
            using (var stream = entry.Open())
            {
                JsonSerializer.Serialize(stream, this);
            }
        }
    }

    public static Project ReadLoaded()
    {
        var ret = new Project()
        {
            // Mixer = JaebeMusicStudio3.Core.Mixer.AudioMixer.Current,
            Timeline = JaebeMusicStudio3.Core.Timeline.Timeline.Current
        };
        return ret;
    }

    public static Project ReadFromFile(string path)
    {
        var ret = new Project();
        using (var zip = ZipFile.Open(path, ZipArchiveMode.Read))
        {
            var entry = zip.GetEntry("project.json");
            using (var stream = entry.Open())
            {
                var a = JsonSerializer.Deserialize<object>(stream);
                ret = JsonSerializer.Deserialize<Project>(stream);
            }
        }

        return ret;
    }

    public void Load()
    {
        JaebeMusicStudio3.Core.Timeline.Timeline.Current = this.Timeline;
        Loaded?.Invoke();
    }

    public Timeline.Timeline Timeline { get; set; }

    // public AudioMixer Mixer { get; set; }
    public static event Action? Loaded;
}