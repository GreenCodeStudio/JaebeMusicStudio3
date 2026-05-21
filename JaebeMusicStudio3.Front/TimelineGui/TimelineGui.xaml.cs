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
    private ITimelineItem _movingNode = null;
    private Point? _movingPoint = null;
    private ITimelineItem _selectedNode;

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
                        ((double)RenderingProcess.Current.Position / RenderingProcess.Current.SampleRate -
                         HorizontalScrollBar.Value) /
                        SecondsPerPixel, 0, 0, 0);
            }
            else
            {
                NowMarker.Margin = new Thickness(0, 0, 0, 0);
            }
        };

        this.MouseMove += (sender, args) =>
        {
            if (args.LeftButton == System.Windows.Input.MouseButtonState.Pressed && _movingNode != null)
            {
                var deltaX = args.MouseDevice.GetPosition(this).X - _movingPoint.Value.X;
                var deltaY = args.MouseDevice.GetPosition(this).Y - _movingPoint.Value.Y;
                _movingNode.OffsetSeconds += deltaX * SecondsPerPixel;
                _movingPoint = args.MouseDevice.GetPosition(this);
            }
        };
        this.MouseUp += (sender, args) =>
        {
            _movingNode = null;
            _movingPoint = null;
        };
        KeyDown += (sender, args) =>
        {
            if (args.Key == Key.Delete)
            {
                if(_selectedNode != null)
                {
                    timeline.Remove(_selectedNode);
                    _selectedNode = null;
                }
            }
        };
        MouseDown += (sender, args) =>
        {
            if (_selectedNode != null && !args.Handled)
            {
                _selectedNode = null;
                Render();
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
                VerticalScrollBar.Value -= e.Delta;
                Render();
            }
        }
    }

    private void Render()
    {
        VerticalScrollBar.Maximum = Math.Max(0,
            timeline.Items.Select(x => x.LineNumber + 1).DefaultIfEmpty(0).Max() * HorizontalLineHeight + 30);
        HorizontalScrollBar.Maximum = timeline.TotalLength;
        this.TimelineMarker.SecondsPerPixel = SecondsPerPixel;
        this.TimelineMarker.OffsetInSeconds = HorizontalScrollBar.Value;
        var maxWidth = this.ActualWidth * SecondsPerPixel;
        Lines.Children.Clear();

        foreach (var item in timeline.Items)
        {
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
                HorizontalLineHeight, length);

            control.Margin = new Thickness((offsetRelative > 0) ? (offsetRelative / SecondsPerPixel) : 0,
                item.LineNumber * HorizontalLineHeight + 30 - VerticalScrollBar.Value, 0, 0);

            control.HorizontalAlignment = HorizontalAlignment.Left;
            control.Width = length / SecondsPerPixel;
            if (control.Width < 16)
            {
                control.Width = 16;
            }

            control.Height = HorizontalLineHeight;
            control.HorizontalAlignment = HorizontalAlignment.Left;
            control.VerticalAlignment = VerticalAlignment.Top;
            control.MouseDown += (sender, args) =>
            {
                if (_selectedNode == item)
                {
                    _movingNode = item;
                    _movingPoint = args.MouseDevice.GetPosition(this);
                    args.Handled = true;
                }
                else
                {
                    _selectedNode = item;
                    args.Handled = true;
                    Render();
                }
            };
            if (_selectedNode == item)
            {
                control.BorderBrush = Brushes.Red;
                control.BorderThickness = new Thickness(2);
            }

            Lines.Children.Add(control);
            item.Changed -= OnItemChanged;
            item.Changed += OnItemChanged;
        }
    }

    private void OnItemChanged()
    {
        Dispatcher.Invoke(Render);
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

    private void TimelineMarker_OnTimeClicked(object? sender, double e)
    {
        RenderingProcess.Current = new RenderingProcess(true, RenderingProcess.Current.WaveFormat,
            (long)(e * RenderingProcess.Current.SampleRate));
    }

    private void NoteLineClick(object sender, RoutedEventArgs e)
    {
        Timeline.Current.Add(new NoteLine()
        {
            Name = "New Line", Notes = new List<Note>()
            {
                new Note()
                {
                    Pitch = 440,
                    Volume = 1,
                    Start = 0,
                    Length = 1
                }
            }
        });
    }

    private void ShowNotesView(object sender, RoutedEventArgs e)
    {
        UiWindow.Open(() =>
            new TabView(() => new TimelineNotesView(timeline)
            )
        );
    }
}