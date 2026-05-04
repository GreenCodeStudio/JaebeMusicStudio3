namespace JaebeMusicStudio3.Core.IO;

public class IOWrapper
{
    public static List<INotesInput> NotesInputs { get; } = new List<INotesInput>();

    public static void Add(INotesInput x)
    {
        NotesInputs.Add(x);
    }
}