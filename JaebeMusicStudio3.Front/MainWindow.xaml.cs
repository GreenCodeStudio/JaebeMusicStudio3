using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using JaebeMusicStudio3.Core.AudioNodes;
using JaebeMusicStudio3.Core.AudioRendering;
using JaebeMusicStudio3.Core.IO;
using JaebeMusicStudio3.Core.Mixer;
using JaebeMusicStudio3.Core.Timeline;
using JaebeMusicStudio3.Front.IO;
using NAudio.CoreAudioApi;
using NAudio.Wave;

namespace JaebeMusicStudio3.Front;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private static WaveProvider provider;
    static private WasapiOut output;

    static MainWindow()
    {
        output = new WasapiOut(AudioClientShareMode.Shared, 0);
        IOWrapper.Add(KeyboardInput.singleton1);
        IOWrapper.Add(KeyboardInput.singleton2);
    }

    public MainWindow()
    {
        InitializeComponent();
        var thread = new Thread(LiveRenderThread);
        thread.Name = "LiveRenderThread";
        thread.Start();

        var enumerator = new MMDeviceEnumerator();
        foreach (var device in enumerator.EnumerateAudioEndPoints(DataFlow.Render, DeviceState.Active))
        {
            var item = new ComboBoxItem();
            item.Tag = device;
            item.Content = device;
            OutputSelect.Items.Add(item);
        }

        OutputSelect.SelectionChanged += (x, y) =>
        {
            output.Stop();
            var device = (OutputSelect.SelectedItem as ComboBoxItem).Content as MMDevice;
            output = new WasapiOut(device, AudioClientShareMode.Shared, true, 0);
            output.Init((IWaveProvider)provider);
            output.Play();

            // foreach (var VARIABLE in device.)
            // {
            //     
            // }
            // QualitySelect.Items.
        };

        QualitySelect.Items.Add(new WaveFormat(8000, 1));
        QualitySelect.Items.Add(new WaveFormat(22100, 1));
        QualitySelect.Items.Add(new WaveFormat(48000, 1));
        QualitySelect.Items.Add(new WaveFormat(4 * 48000, 1));
        QualitySelect.Items.Add(new WaveFormat(8000, 2));
        QualitySelect.Items.Add(new WaveFormat(22100, 2));
        QualitySelect.Items.Add(new WaveFormat(48000, 2));
        QualitySelect.Items.Add(new WaveFormat(4 * 48000, 2));
        QualitySelect.Items.Add(new WaveFormat(48000, 4));
        QualitySelect.SelectedItem = new WaveFormat(48000, 2);
        QualitySelect.SelectionChanged += (x, y) =>
        {
            output.Stop();
            provider.WaveFormat = (WaveFormat)QualitySelect.SelectedItem;
            RenderingProcess.Current.WaveFormat = provider.WaveFormat;
            var device = (OutputSelect.SelectedItem as ComboBoxItem).Content as MMDevice;
            output = new WasapiOut(device, AudioClientShareMode.Shared, true, 0);
            output.Init((IWaveProvider)provider);
            output.Play();
        };
    }

    private static void LiveRenderThread()
    {
        var mixer = AudioMixer.Current;
        var timeline = Timeline.Current;
        var dummy = new DummyNoise();
        var volume = new VolumeNode();
        var input = new LiveAudioInput();

        mixer.Add(dummy);
        mixer.Add(input);
        mixer.Add(volume);
        // mixer.Connect(dummy.Outputs.First(), volume.Inputs.First());
        // mixer.Connect(input.Outputs.First(), volume.Inputs.First());
        mixer.MainOutput = volume.Outputs.First();

        var instrument = new Instrument();
        mixer.Add(instrument);
        var noteLine = new NoteLine()
        {
            Instrument = instrument,
            Notes = new List<Note>()
            {
                new Note()
                {
                    Start = 0,
                    Length = 0.5,
                    Pitch = 440
                },
                new Note()
                {
                    Start = 1,
                    Length = 0.5,
                    Pitch = 440
                },
                new Note()
                {
                    Start = 2,
                    Length = 0.5,
                    Pitch = 440 * Math.Pow(2, 4.0 / 12)
                }
            }
        };
        timeline.Add(noteLine);

        var process = RenderingProcess.Current;
        MainWindow.provider = new WaveProvider(mixer);
        provider.WaveFormat = new WaveFormat(48000, 2);
        output.Init((IWaveProvider)provider);
        output.Play();

        UiWindow.Open(() =>
            new TabView(
                () => new MixerGui.MixerGui(mixer),
                () => new TimelineGui.TimelineGui(timeline),
                () => new IOGui()
            )
        );
    }
}