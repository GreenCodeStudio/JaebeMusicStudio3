using System.Windows.Controls;
using System.Windows.Shapes;

namespace JaebeMusicStudio3.Front.TimelineGui;

public partial class TimelineMarkerBeats : UserControl
{
    public TimelineMarkerBeats()
    {
        InitializeComponent();
        Render();
    }


    public double BeatsPerPixel
    {
        get;
        set
        {
            field = value;
            Render();
        }
    } = 0.01;

    public double Offset
    {
        get;
        set
        {
            field = value;
            Render();
        }
    } = 0;

    public double LengthInPixels
    {
        get;
        set
        {
            field = value;
            Render();
        }
    } = 1000;

    public bool Lines { get; set; } = false;
    public bool Texts { get; set; } = true;

    private void Render()
    {
        MainGrid.Children.Clear();
        var step = getStep();
        var length = ActualWidth * BeatsPerPixel;
        for (var i = Math.Floor(Offset / step) * step;
             i <= length + Offset;
             i += step)
        {
            if (Texts)
            {
                var label = new Label();
                label.Content = formatByStep(i, step);
                label.Margin = new System.Windows.Thickness((i - Offset) / BeatsPerPixel, 0, 0, 0);
                label.VerticalAlignment = System.Windows.VerticalAlignment.Top;
                label.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                MainGrid.Children.Add(label);
            }

            if (Lines)
            {
                var line = new Rectangle();
                line.Fill = System.Windows.Media.Brushes.Black;
                line.Width = 1;
                line.Margin = new System.Windows.Thickness((i - Offset) / BeatsPerPixel, 0, 0, 0);
                line.VerticalAlignment = System.Windows.VerticalAlignment.Stretch;
                line.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                MainGrid.Children.Add(line);
            }
        }
    }

    private double getStep()
    {
        var basePixelsStep = 100;
        if (basePixelsStep * BeatsPerPixel <= 1)
        {
            var stepInSeconds1 = Math.Pow(10, Math.Ceiling(Math.Log10(basePixelsStep * BeatsPerPixel)));
            var stepInSeconds2 = Math.Pow(10, Math.Ceiling(Math.Log10(basePixelsStep * BeatsPerPixel * 2))) / 2;
            var stepInSeconds5 = Math.Pow(10, Math.Ceiling(Math.Log10(basePixelsStep * BeatsPerPixel * 5))) / 5;
            return Math.Min(stepInSeconds1, Math.Min(stepInSeconds2, stepInSeconds5));
        }
        else
        {
            var bases = new double[] { 1, 4, 16, 32, 64, 128, 256 };
            foreach (var b in bases)
            {
                if (b / BeatsPerPixel > basePixelsStep)
                {
                    return b;
                }
            }

            return bases.Last();
        }
    }

    private string formatByStep(double value, double step)
    {
        var ret = "";

        if (step < 1)
        {
            var digits = Math.Ceiling(-Math.Log10(step));
            ret += value.ToString("F" + digits);
        }
        else
        {
            ret += value.ToString();
        }

        return ret;
    }
}