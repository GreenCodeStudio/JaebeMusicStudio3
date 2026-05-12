using System.IO.Compression;
using System.Text.Json;
using JaebeMusicStudio3.Core.Mixer;

namespace JaebeMusicStudio3.Core.Serialization;

public class Project
{
    public void SaveAsFile(string path)
    {
        using(var zip=ZipFile.Open(path, ZipArchiveMode.Create))
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

    public Timeline.Timeline Timeline { get; set; }

    // public AudioMixer Mixer { get; set; }
}