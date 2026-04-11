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
using JaebeMusicStudio3.Core.Mixer;
using JaebeMusicStudio3.Core.Timeline;
using NAudio.CoreAudioApi;
using NAudio.Wave;

namespace JaebeMusicStudio3.Front;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        var thread = new Thread(LiveRenderThread);
        thread.Start();
    }

    private static void LiveRenderThread()
    {
        var mixer = new AudioMixer();
        var timeline = new Timeline();
        var dummy = new DummyNoise();
        var volume = new VolumeNode();
        var input = new LiveAudioInput();

        mixer.Add(dummy);
        mixer.Add(input);
        mixer.Add(volume);
        // mixer.Connect(dummy.Outputs.First(), volume.Inputs.First());
        // mixer.Connect(input.Outputs.First(), volume.Inputs.First());
        mixer.MainOutput = volume.Outputs.First();

        var process = new RenderingProcess();
        var provider = new WaveProvider(process, mixer);
        provider.WaveFormat = new WaveFormat(48000, 2);
        var output = new WasapiOut(AudioClientShareMode.Shared, 0);
        output.Init((IWaveProvider)provider);
        output.Play();

        UiWindow.Open(() => new MixerGui.MixerGui(mixer));
        UiWindow.Open(() => new TimelineGui.TimelineGui(timeline));
    }
}