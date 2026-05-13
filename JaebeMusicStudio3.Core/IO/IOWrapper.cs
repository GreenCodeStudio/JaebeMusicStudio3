using JaebeMusicStudio3.Core.AudioNodes;

namespace JaebeMusicStudio3.Core.IO;

public class IOWrapper
{
    public static List<INotesInput> NotesInputs { get; } = new List<INotesInput>();

    public static void Add(INotesInput x)
    {
        NotesInputs.Add(x);
    }

    public static IEnumerable<NotesInputAssignment> Assignments
    {
        get
        {
            return NotesInputs.Select(x => new NotesInputAssignment() { Input = x.Id, Instrument = x.Instrument?.Id });
        }
        set
        {
            foreach (var notesInput in NotesInputs)
            {
                notesInput.Instrument = Mixer.AudioMixer.Current.Nodes.FirstOrDefault(x =>
                    x.Id == value.FirstOrDefault(y => y.Input == notesInput.Id)?.Instrument) as Instrument;
            }
            Changed?.Invoke();
        }
    }
    
    public static event Action Changed;
}