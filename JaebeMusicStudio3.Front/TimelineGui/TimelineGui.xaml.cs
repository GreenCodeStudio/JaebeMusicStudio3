using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
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
    private double HorizontalLineHeight = 100.0;

    public TimelineGui(Timeline timeline)
    {
        this.timeline = timeline;
        InitializeComponent();
        this.AllowDrop = true;
        Drop += OnDrop;
        Render();
        timeline.Changed += () => { Dispatcher.Invoke(() => Render()); };
        this.SizeChanged += (_, _) => { Dispatcher.Invoke(() => Render()); };
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
            if (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift))
            {
                SecondsPerPixel *= multiplier;
            }
            else
            {
                HorizontalLineHeight *= multiplier;
            }

            Render();
        }
        else
        {
            if (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift))
            {
                HorizontalScrollBar.Value -= e.Delta * SecondsPerPixel;
                Render();
            }
            else
            {
                
            }
        }
    }

    private void Render()
    {
        HorizontalScrollBar.Maximum = timeline.TotalLength;
        this.TimelineMarker.SecondsPerPixel = SecondsPerPixel;
        this.TimelineMarker.OffsetInSeconds = HorizontalScrollBar.Value;
        var maxWidth = this.ActualWidth * SecondsPerPixel;
        while (Lines.Children.Count > 2)
        {
            Lines.Children.RemoveAt(1);
        }

        foreach (var item in timeline.Items)
        {
            var line = new Grid();
            line.Height = HorizontalLineHeight;
            Lines.Children.Insert(Lines.Children.Count - 1, line);
            var offsetRelative = item.OffsetSeconds - HorizontalScrollBar.Value;

            var length = item.LengthSeconds;
            var endOffset = offsetRelative + length;
            if (offsetRelative < 0)
            {
                length += offsetRelative;
            }

            if (endOffset > maxWidth)
            {
                length -= endOffset - maxWidth;
            }

            if (length <= 0)
                continue;
            var control = new TimelineItemGui(item, SecondsPerPixel, offsetRelative > 0 ? 0 : offsetRelative,
                HorizontalLineHeight);
            if (offsetRelative > 0)
                control.Margin = new Thickness(offsetRelative / SecondsPerPixel, 0, 0, 0);

            control.HorizontalAlignment = HorizontalAlignment.Left;
            control.Width = length / SecondsPerPixel;
            line.Children.Add(control);
            item.Changed += () => Dispatcher.Invoke(Render);
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

    private void ZoomHOut(object sender, RoutedEventArgs e)
    {
        HorizontalLineHeight /= 1.25;
        Render();
    }

    private void ZoomHIn(object sender, RoutedEventArgs e)
    {
        HorizontalLineHeight *= 1.25;
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

    private void ScrollBar_OnScroll(object sender, ScrollEventArgs e)
    {
        Render();
    }
}