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
using JaebeMusicStudio3.Core.Serialization;
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
        output = new WasapiOut(AudioClientShareMode.Shared, 10);
        IOWrapper.Add(KeyboardInput.singleton1);
        IOWrapper.Add(KeyboardInput.singleton2);
    }

    public MainWindow()
    {
        InitializeComponent();
        Tabs.Children.Add(new TabView(() => new Welcome()));
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
        var volume = new VolumeNode()
        {
            Volume = 0.01f
        };

        mixer.Add(volume);
        var mod2 = new NoteModificationNode()
        {
            Multipler = 1.001
        };
        var mod3 = new NoteModificationNode()
        {
            Multipler = .998
        };
        var mod4 = new NoteModificationNode()
        {
            Multipler = 1.003
        };
        var oscilator = new BasicOscillatorNode()
        {
            Shape = BasicOscillatorNode.WaveShape.Square
        };
        var oscilator2 = new BasicOscillatorNode()
        {
            Shape = BasicOscillatorNode.WaveShape.Square
        };
        var oscilator3 = new BasicOscillatorNode()
        {
            Shape = BasicOscillatorNode.WaveShape.Square
        };
        var oscilator4 = new BasicOscillatorNode()
        {
            Shape = BasicOscillatorNode.WaveShape.Square
        };
        var oscMixer = new MixNode();
        var oscMixer2 = new MixNode();
        var oscMixer3 = new MixNode();
        var instrument = new Instrument()
        {
            MainOutput = oscMixer3.Outputs.First(),
        };
        instrument.Add(mod2);
        instrument.Add(mod3);
        instrument.Add(mod4);
        instrument.Add(oscilator);
        instrument.Add(oscilator2);
        instrument.Add(oscilator3);
        instrument.Add(oscilator4);
        instrument.Add(oscMixer);
        instrument.Add(oscMixer2);
        instrument.Add(oscMixer3);
        instrument.Connect(mod2.Outputs.First(), oscilator2.Inputs.First());
        instrument.Connect(mod3.Outputs.First(), oscilator3.Inputs.First());
        instrument.Connect(mod4.Outputs.First(), oscilator4.Inputs.First());
        instrument.Connect(oscilator.Outputs.First(), oscMixer.Inputs.First());
        instrument.Connect(oscilator2.Outputs.First(), oscMixer.Inputs.Skip(1).First());
        instrument.Connect(oscilator3.Outputs.First(), oscMixer2.Inputs.First());
        instrument.Connect(oscilator4.Outputs.First(), oscMixer2.Inputs.Skip(1).First());
        instrument.Connect(oscMixer.Outputs.First(), oscMixer3.Inputs.First());
        instrument.Connect(oscMixer2.Outputs.First(), oscMixer3.Inputs.Skip(1).First());
        instrument.ReorganizePositions();
        mixer.Add(instrument);
        mixer.MainOutput = volume.Outputs.First();
        // mixer.Connect(instrument.MainOutput, volume.Inputs.First());
        mixer.ReorganizePositions();
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
        Project.Loaded += () =>
        {
            UiWindow.Open(() =>
                new TabView(
                    () => new MixerGui.MixerGui(AudioMixer.Current),
                    () => new TimelineGui.TimelineGui(Timeline.Current),
                    () => new IOGui()
                )
            );
        };
    }
}