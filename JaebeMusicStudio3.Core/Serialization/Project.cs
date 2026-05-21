using System.IO.Compression;
using System.Text.Json;
using JaebeMusicStudio3.Core.AudioNodes;
using JaebeMusicStudio3.Core.IO;
using JaebeMusicStudio3.Core.Mixer;
using JaebeMusicStudio3.Core.Timeline;

namespace JaebeMusicStudio3.Core.Serialization;

public class Project
{
    public void SaveAsFile(string path)
    {
        if (File.Exists(path))
        {
            File.Delete(path);
        }

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
            Mixer = JaebeMusicStudio3.Core.Mixer.AudioMixer.Current,
            Timeline = JaebeMusicStudio3.Core.Timeline.Timeline.Current,
            IoAssignments = IOWrapper.Assignments
        };
        return ret;
    }

    public IEnumerable<NotesInputAssignment> IoAssignments { get; set; }

    public static Project ReadFromFile(string path)
    {
        var ret = new Project();
        using (var zip = ZipFile.Open(path, ZipArchiveMode.Read))
        {
            var entry = zip.GetEntry("project.json");
            using (var stream = entry.Open())
            {
                ret = JsonSerializer.Deserialize<Project>(stream);
            }
        }

        return ret;
    }

    public void Load()
    {
        JaebeMusicStudio3.Core.Timeline.Timeline.Current = this.Timeline;
        AudioMixer.Current = this.Mixer;
        IOWrapper.Assignments = this.IoAssignments;
        foreach (var timelineItem in Timeline.Items)
        {
            if (timelineItem is NoteLine tin)
            {
                tin.Instrument = Mixer.Nodes.OfType<Instrument>().FirstOrDefault(i => i.Id == tin.InstrumentId);
            }
        }

        Loaded?.Invoke();
    }

    public Timeline.Timeline Timeline { get; set; }

    public AudioMixer Mixer { get; set; }
    public static event Action? Loaded;
}