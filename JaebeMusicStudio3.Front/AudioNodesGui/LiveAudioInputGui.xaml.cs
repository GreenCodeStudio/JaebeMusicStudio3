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
        QualitySelect.Items.Add(new WaveFormat(8000, 1));
        QualitySelect.Items.Add(new WaveFormat(22100, 1));
        QualitySelect.Items.Add(new WaveFormat(48000, 1));
        QualitySelect.Items.Add(new WaveFormat(4 * 48000, 1));
        QualitySelect.Items.Add(new WaveFormat(8000, 2));
        QualitySelect.Items.Add(new WaveFormat(22100, 2));
        QualitySelect.Items.Add(new WaveFormat(48000, 2));
        QualitySelect.Items.Add(new WaveFormat(4 * 48000, 2));
        QualitySelect.Items.Add(new WaveFormat(48000, 4));
        QualitySelect.SelectedItem = liveAudioInput.WaveFormat;
        QualitySelect.SelectionChanged += (x, y) =>
        {
            liveAudioInput.WaveFormat = (WaveFormat)QualitySelect.SelectedItem;
        };
    }

    public LiveAudioInput LiveAudioInput { get; set; }
}