using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using JaebeMusicStudio3.Core.AudioNodes;
using JaebeMusicStudio3.Core.Mixer;
using JaebeMusicStudio3.Core.Timeline;

namespace JaebeMusicStudio3.Front.TimelineGui.NoteLineEditor;

public partial class NoteLineEditor : UserControl
{
    private readonly NoteLine item;
    private readonly HorizontalNoteEditor NoteEditor;

    public NoteLineEditor(NoteLine itemN)
    {
        InitializeComponent();
        this.item = itemN;
        InstrumentSelect.Items.Add("Empty");
        foreach (var x in AudioMixer.Current.Nodes.Where(n => n is Instrument))
        {
            InstrumentSelect.Items.Add(x);
        }

        InstrumentSelect.SelectionChanged += (s, e) =>
        {
            if (InstrumentSelect.SelectedItem != null)
            {
                var instrument = InstrumentSelect.SelectedItem as Instrument;
                item.Instrument = instrument;
            }
        };
        this.NoteEditor = new HorizontalNoteEditor(()=>itemN.Notes, ()=>itemN.Tempo);
        NoteEditor.NoteAdded += (n) =>
        {
            itemN.Notes.Add(n);
            itemN.InvokeChanged();
        };
        NoteEditor.NoteChanged += () => itemN.InvokeChanged();
        NoteEditorWrapper.Children.Add(NoteEditor);
    }
}