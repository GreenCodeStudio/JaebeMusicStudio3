using System.Windows.Controls;
using JaebeMusicStudio3.Core.AudioNodes;
using NAudio.CoreAudioApi;
using NAudio.Wave;

namespace JaebeMusicStudio3.Front.AudioNodesGui;

public partial class LiveAudioInputGui : UserControl
{
    public LiveAudioInputGui(LiveAudioInput liveAudioInput)
    {
        this.LiveAudioInput = liveAudioInput;
        InitializeComponent();
        var enumerator = new MMDeviceEnumerator();
        foreach (var device in enumerator.EnumerateAudioEndPoints(DataFlow.Capture, DeviceState.Active))
        {
            var item = new ComboBoxItem();
            item.Tag = device;
            item.Content = device;
            InputSelect.Items.Add(item);
        }

        InputSelect.SelectionChanged += (x, y) =>
        {
            LiveAudioInput.Device = (InputSelect.SelectedItem as ComboBoxItem).Content as MMDevice;
        };
        InputSelect.SelectedItem = liveAudioInput.Device;
    }

    public LiveAudioInput LiveAudioInput { get; set; }
}