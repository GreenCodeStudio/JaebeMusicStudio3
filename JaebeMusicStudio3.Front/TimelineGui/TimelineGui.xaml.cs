using System.Windows;
using System.Windows.Controls;
using JaebeMusicStudio3.Core.Timeline;
using NAudio.Wave;

namespace JaebeMusicStudio3.Front.TimelineGui;

public partial class TimelineGui : UserControl
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
    }

    private void Render()
    {
        while (Lines.Children.Count > 2)
        {
            Lines.Children.RemoveAt(1);
        }
        foreach(var item in timeline.Items)
        {
            if (item is RecordedSoundInProgress recordedSoundInProgress)
            {
                var line = new Grid();
                line.Height = 100;
                line.Width=recordedSoundInProgress.LengthSeconds / SecondsPerPixel;
                Lines.Children.Insert(Lines.Children.Count - 1, line);
                var control = new TimelineItemGui(recordedSoundInProgress, SecondsPerPixel);
                line.Children.Add(control);
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
}