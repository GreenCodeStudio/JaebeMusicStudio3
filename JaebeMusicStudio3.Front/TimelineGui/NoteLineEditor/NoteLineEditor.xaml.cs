using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using JaebeMusicStudio3.Core.Timeline;

namespace JaebeMusicStudio3.Front.TimelineGui.NoteLineEditor;

public partial class NoteLineEditor : UserControl
{
    private double PitchLog = Math.Log2(440) * 12;
    private double PitchNoteHeight = 50;
    private double BeatsPerPixel = 0.01;
    private readonly NoteLine item;
    private Note _movingNode = null;
    private Point? _movingPoint = null;

    public NoteLineEditor(NoteLine itemN)
    {
        this.item = itemN;
        InitializeComponent();
        SizeChanged += (_, _) => Render();
        MouseWheel += OnMouseWheel;

        this.MouseMove += (sender, args) =>
        {
            if (args.LeftButton == System.Windows.Input.MouseButtonState.Pressed && _movingNode != null)
            {
                var cNoteLog = Math.Log2(440.0) * 12;
                var deltaX = args.MouseDevice.GetPosition(this).X - _movingPoint.Value.X;
                var deltaY = args.MouseDevice.GetPosition(this).Y - _movingPoint.Value.Y;
                _movingNode.Start += deltaX * BeatsPerPixel;
                var pitchLog = Math.Log2(_movingNode.Pitch) * 12 - cNoteLog;
                var newPitchLog = Math.Round(pitchLog - deltaY / PitchNoteHeight);
                _movingNode.Pitch = Math.Pow(2, (newPitchLog + cNoteLog) / 12);
                _movingPoint = new Point(_movingPoint.Value.X + deltaX,
                    _movingPoint.Value.Y + (pitchLog - newPitchLog) * PitchNoteHeight);
                RenderPlane();
                item.InvokeChanged();
            }
        };
        this.MouseUp += (sender, args) =>
        {
            _movingNode = null;
            _movingPoint = null;
        };
    }

    private void OnMouseWheel(object sender, MouseWheelEventArgs e)
    {
        if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
        {
            var multiplier = Math.Pow(1.25, e.Delta / 120.0);
            if (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift))
            {
                // SecondsPerPixel *= multiplier;
            }
            else
            {
                PitchNoteHeight *= multiplier;
            }
        }
        else
        {
            if (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift))
            {
                // HorizontalScrollBar.Value -= e.Delta * SecondsPerPixel;
            }
            else
            {
                PitchLog += e.Delta / 120.0;
            }
        }

        Render();
    }

    private void Render()
    {
        RenderPlane();
        RenderPitches();
    }

    private void RenderPitches()
    {
        var noteName = new string[] { "C", "C#", "D", "D#", "E", "F", "F#", "G", "G#", "A", "A#", "B" };
        var noteBlack = new bool[] { false, true, false, true, false, false, true, false, true, false, true, false };
        Pitches.Children.Clear();
        var cNoteLog = Math.Log2(440.0) * 12;
        var noteLength = ActualHeight / PitchNoteHeight;
        var startLogPitch = Math.Floor(PitchLog - cNoteLog);
        for (var i = startLogPitch; i > startLogPitch - noteLength; i--)
        {
            var noteModulo = (int)Math.Round(i % 12);
            if (noteModulo < 0)
                noteModulo += 12;
            var label = new Label();
            label.Content = i + " " + (Math.Pow(2, (i + cNoteLog) / 12)) + " " + noteName[noteModulo];
            label.Background = noteBlack[noteModulo]
                ? new SolidColorBrush(Color.FromArgb(255, 200, 200, 200))
                : new SolidColorBrush(Color.FromArgb(255, 220, 220, 220));
            label.Height = PitchNoteHeight;
            Pitches.Children.Add(label);
            label.VerticalAlignment = VerticalAlignment.Top;
            label.Margin = new Thickness(0, (startLogPitch - i) * PitchNoteHeight, 0, 0);
        }
    }

    private void RenderPlane()
    {
        Plane.Children.Clear();
        foreach (var n in item.Notes)
        {
            var pitch = Math.Log2(n.Pitch) * 12;
            var rect = new Rectangle();
            rect.VerticalAlignment = VerticalAlignment.Top;
            rect.HorizontalAlignment = HorizontalAlignment.Left;
            rect.Margin = new Thickness((n.Start) / BeatsPerPixel,
                (PitchLog - pitch) * PitchNoteHeight, 0, 0);
            rect.Width = n.Length / BeatsPerPixel;
            rect.Height = PitchNoteHeight;
            rect.Fill = System.Windows.Media.Brushes.Green;
            rect.Stroke = System.Windows.Media.Brushes.Black;
            Plane.Children.Add(rect);
            rect.MouseDown += (sender, args) =>
            {
                _movingNode = n;
                _movingPoint = args.MouseDevice.GetPosition(this);
            };
        }
    }
}