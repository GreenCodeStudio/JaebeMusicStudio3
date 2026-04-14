using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using JaebeMusicStudio3.Core.AudioRendering;
using JaebeMusicStudio3.Core.Mixer;
using JaebeMusicStudio3.Core.Timeline;
using NAudio.Wave;

namespace JaebeMusicStudio3.Front.TimelineGui;

public partial class TimelineGui : UserControl, ITabbableControl
{
    private readonly Timeline timeline;
    private double SecondsPerPixel = 0.01;

    public TimelineGui(Timeline timeline)
    {
        this.timeline = timeline;
        InitializeComponent();
        this.AllowDrop = true;
        Drop += OnDrop;
        Render();
        timeline.Changed += () => { Dispatcher.Invoke(() => Render()); };
        MouseWheel += TimelineGui_MouseWheel;
        CompositionTarget.Rendering += (s, e) =>
        {
            if (RenderingProcess.Current.UseTimeline)
            {
                NowMarker.Margin =
                    new Thickness(
                        (double)RenderingProcess.Current.Position / RenderingProcess.Current.SampleRate /
                        SecondsPerPixel, 0, 0, 0);
            }
            else
            {
                NowMarker.Margin = new Thickness(0, 0, 0, 0);
            }
        };
    }

    private void TimelineGui_MouseWheel(object sender, MouseWheelEventArgs e)
    {
        if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
        {
            var multiplier = Math.Pow(1.25, e.Delta / 120.0);
            SecondsPerPixel *= multiplier;
            Render();
        }
    }

    private void Render()
    {
        this.TimelineMarker.SecondsPerPixel = SecondsPerPixel;
        while (Lines.Children.Count > 2)
        {
            Lines.Children.RemoveAt(1);
        }

        foreach (var item in timeline.Items)
        {
            if (item is RecordedSoundInProgress recordedSoundInProgress)
            {
                var line = new Grid();
                line.Height = 100;
                Lines.Children.Insert(Lines.Children.Count - 1, line);
                var control = new TimelineItemGui(recordedSoundInProgress, SecondsPerPixel);
                control.HorizontalAlignment = HorizontalAlignment.Left;
                control.Width = recordedSoundInProgress.LengthSeconds / SecondsPerPixel;
                line.Children.Add(control);
                recordedSoundInProgress.Changed += ()=>Dispatcher.Invoke(Render);
            }else if (item is RecordedSound recordedSound)
            {
                var line = new Grid();
                line.Height = 100;
                Lines.Children.Insert(Lines.Children.Count - 1, line);
                var control = new TimelineItemGui(recordedSound, SecondsPerPixel);
                control.HorizontalAlignment = HorizontalAlignment.Left;
                control.Width = recordedSound.LengthSeconds / SecondsPerPixel;
                line.Children.Add(control);
                recordedSound.Changed += ()=>Dispatcher.Invoke(Render);
            }
        }
    }

    private void OnDrop(object sender, DragEventArgs e)
    {
        var fileNames = e.Data.GetData("FileDrop");

        foreach (var x in fileNames as string[])
        {
            timeline.LoadFileAsync(x);
        }
    }

    private void LoadFileClick(object sender, RoutedEventArgs e)
    {
        var dialog = new Microsoft.Win32.OpenFileDialog();
        dialog.Multiselect = true;

        bool? result = dialog.ShowDialog();

        if (result == true)
        {
            foreach (var fileName in dialog.FileNames)
            {
                timeline.LoadFileAsync(fileName);
            }
        }
    }

    private void AudioRecorderClick(object sender, RoutedEventArgs e)
    {
        var recorder = new AudioRecorder();
        timeline.Add(recorder.RecordedSoundInProgress);
        AudioMixer.Current.Add(recorder.RecordedSoundInProgress);
        UiWindow.Open(() => new AudioRecorderGui(recorder));
    }

    private void ZoomOut(object sender, RoutedEventArgs e)
    {
        SecondsPerPixel *= 1.25;
        Render();
    }

    private void ZoomIn(object sender, RoutedEventArgs e)
    {
        SecondsPerPixel /= 1.25;
        Render();
    }

    private void Play(object sender, RoutedEventArgs e)
    {
        RenderingProcess.Current = new RenderingProcess(true, RenderingProcess.Current.WaveFormat);
    }

    private void Stop(object sender, RoutedEventArgs e)
    {
        RenderingProcess.Current = new RenderingProcess(false, RenderingProcess.Current.WaveFormat);
    }

    public string Title => "Timeline";
    public event Action? ChangedMetadata;
}