using System.Windows;
using System.Windows.Controls;
using JaebeMusicStudio3.Core.Timeline;
using NAudio.CoreAudioApi;

namespace JaebeMusicStudio3.Front.TimelineGui;

public partial class AudioRecorderGui : UserControl
{
    private readonly AudioRecorder _recorder;
    private bool _isRecording;

    public AudioRecorderGui(AudioRecorder recorder)
    {
        this._recorder = recorder;
        InitializeComponent();
        var enumerator = new MMDeviceEnumerator();
        foreach (var device in enumerator.EnumerateAudioEndPoints(DataFlow.Capture, DeviceState.Active))
        {
            var item = new ComboBoxItem();
            item.Tag = device;
            item.Content = device;
            InputSelect.Items.Add(item);
        }
    }

    private void StartStop_Click(object sender, RoutedEventArgs e)
    {
        if (!_isRecording)
        {
            _recorder.Device = InputSelect.SelectedItem is ComboBoxItem item ? item.Content as MMDevice : null;
            _recorder.StartRecording();
            _isRecording = true;
        }
        else
        {
            _recorder.StopRecording();
            //Close window
        }
    }
}