using System.Windows.Controls;
using System.Windows.Media;
using JaebeMusicStudio3.Core.Timeline;

namespace JaebeMusicStudio3.Front.Controls;

public partial class NoteVisualization : UserControl
{
    public Func<List<Note>> GetNotes { get; set; } = (() => new List<Note>());

    public NoteVisualization()
    {
        InitializeComponent();
        CompositionTarget.Rendering += (s, e) => { Dispatcher.Invoke(() => Render()); };
    }

    void Render()
    {
        var notes = GetNotes();
        tmp.Content = string.Join("\n",
            notes.Select(n => $"{n.Pitch} {n.Pitch / 261.63} {Math.Log2(n.Pitch / 261.63) * 12}"));
        Octave0.Notes = notes;
        Octave1.Notes = notes;
        Octave2.Notes = notes;
        Octave3.Notes = notes;
        Octave4.Notes = notes;
        Octave5.Notes = notes;
        Octave6.Notes = notes;
    }
}