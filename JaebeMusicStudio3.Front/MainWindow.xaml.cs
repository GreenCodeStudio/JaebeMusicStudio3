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
        var mixer = new Mixer();
        var dummy = new DummyNoise();
        var input = new LiveAudioInput(0);

        mixer.Add(dummy);
        mixer.Add(input);
        mixer.MainOutput = input.Outputs.First();

        var process = new RenderingProcess();
        var provider = new WaveProvider(process, mixer);
        provider.WaveFormat = new WaveFormat(48000, 2);
        var output = new WasapiOut(AudioClientShareMode.Shared, 100);
        output.Init((IWaveProvider)provider);
        output.Play();

    }
}