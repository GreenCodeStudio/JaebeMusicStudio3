using System.Windows;
using System.Windows.Controls;
using JaebeMusicStudio3.Core.AudioNodes;
using JaebeMusicStudio3.Front.MixerGui;
using NAudio.CoreAudioApi;
using NAudio.Wave;

namespace JaebeMusicStudio3.Front.AudioNodesGui;

public partial class InstrumentNodeGui : UserControl
{
    public InstrumentNodeGui(Instrument item)
    {
        this.Item = item;
        InitializeComponent();
    }

    public Instrument Item { get; set; }

    private void OpenEditor(object sender, RoutedEventArgs e)
    {
        UiWindow.Open(() =>
            new TabView(() => new InstrumentEditorGui(Item)
            )
        );
    }
}