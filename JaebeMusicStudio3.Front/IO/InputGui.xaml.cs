using System.Windows.Controls;
using JaebeMusicStudio3.Core.AudioNodes;
using JaebeMusicStudio3.Core.IO;
using JaebeMusicStudio3.Core.Mixer;
using NAudio.Mixer;

namespace JaebeMusicStudio3.Front.IO;

public partial class InputGui : UserControl
{
    public InputGui(INotesInput item)
    {
        InitializeComponent();
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
    }
}