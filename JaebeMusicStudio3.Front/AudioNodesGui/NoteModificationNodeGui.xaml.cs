using System.Windows;
using System.Windows.Controls;
using JaebeMusicStudio3.Core.AudioNodes;

namespace JaebeMusicStudio3.Front.AudioNodesGui;

public partial class NoteModificationNodeGui : UserControl
{
    public NoteModificationNodeGui(NoteModificationNode noteModificationNode)
    {
        InitializeComponent();
        this.NoteModificationNode = noteModificationNode;
        SetSliders();

        Octave.ValueChanged += ValueChanged;
        Note.ValueChanged += ValueChanged;
        SubNote.ValueChanged += ValueChanged;
    }

    private void ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        Octave.Value = Math.Round(Octave.Value);
        Note.Value = Math.Round(Note.Value);
        var log = Octave.Value + Note.Value / 12 + SubNote.Value / 12;
        NoteModificationNode.Multipler = Math.Pow(2, log);
    }

    public NoteModificationNode NoteModificationNode { get; set; }

    private void SetSliders()
    {
        var log = Math.Log2(NoteModificationNode.Multipler);
        Octave.Value = Math.Round(log);
        log -= Octave.Value;
        log *= 12;
        Note.Value = Math.Round(log);
        log -= Note.Value;
        SubNote.Value = log;
    }
}