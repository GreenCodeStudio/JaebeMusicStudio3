using System.Windows.Controls;
using System.Windows.Media;
using JaebeMusicStudio3.Core.Timeline;

namespace JaebeMusicStudio3.Front.Controls;

public partial class KeyboardOctaveVisualization : UserControl
{
    public double CFrequency { get; set; } = 261.63;

    public List<Note> Notes
    {
        set
        {
            var halfC = CFrequency * Math.Pow(2, -0.5 / 12);
            N0.Fill =
                value.Any(n => n.Pitch > halfC * Math.Pow(2, 0.0 / 12) && n.Pitch <= halfC * Math.Pow(2, 1.0 / 12))
                    ? Brushes.Red
                    : Brushes.White;

            N1.Fill =
                value.Any(n => n.Pitch > halfC * Math.Pow(2, 1.0 / 12) && n.Pitch <= halfC * Math.Pow(2, 2.0 / 12))
                    ? Brushes.Red
                    : Brushes.Black;

            N2.Fill =
                value.Any(n => n.Pitch > halfC * Math.Pow(2, 2.0 / 12) && n.Pitch <= halfC * Math.Pow(2, 3.0 / 12))
                    ? Brushes.Red
                    : Brushes.White;

            N3.Fill =
                value.Any(n => n.Pitch > halfC * Math.Pow(2, 3.0 / 12) && n.Pitch <= halfC * Math.Pow(2, 4.0 / 12))
                    ? Brushes.Red
                    : Brushes.Black;

            N4.Fill =
                value.Any(n => n.Pitch > halfC * Math.Pow(2, 4.0 / 12) && n.Pitch <= halfC * Math.Pow(2, 5.0 / 12))
                    ? Brushes.Red
                    : Brushes.White;

            N5.Fill =
                value.Any(n => n.Pitch > halfC * Math.Pow(2, 5.0 / 12) && n.Pitch <= halfC * Math.Pow(2, 6.0 / 12))
                    ? Brushes.Red
                    : Brushes.White;

            N6.Fill =
                value.Any(n => n.Pitch > halfC * Math.Pow(2, 6.0 / 12) && n.Pitch <= halfC * Math.Pow(2, 7.0 / 12))
                    ? Brushes.Red
                    : Brushes.Black;

            N7.Fill =
                value.Any(n => n.Pitch > halfC * Math.Pow(2, 7.0 / 12) && n.Pitch <= halfC * Math.Pow(2, 8.0 / 12))
                    ? Brushes.Red
                    : Brushes.White;
            N8.Fill =
                value.Any(n => n.Pitch > halfC * Math.Pow(2, 8.0 / 12) && n.Pitch <= halfC * Math.Pow(2, 9.0 / 12))
                    ? Brushes.Red
                    : Brushes.Black;

            N9.Fill = value.Any(n =>
                n.Pitch > halfC * Math.Pow(2, 9.0 / 12) && n.Pitch <= halfC * Math.Pow(2, 10.0 / 12))
                ? Brushes.Red
                : Brushes.White;

            N10.Fill = value.Any(n =>
                n.Pitch > halfC * Math.Pow(2, 10.0 / 12) && n.Pitch <= halfC * Math.Pow(2, 11.0 / 12))
                ? Brushes.Red
                : Brushes.Black;

            N11.Fill = value.Any(n =>
                n.Pitch > halfC * Math.Pow(2, 11.0 / 12) && n.Pitch <= halfC * Math.Pow(2, 12.0 / 12))
                ? Brushes.Red
                : Brushes.White;
        }
    }

    public KeyboardOctaveVisualization()
    {
        InitializeComponent();
    }
}