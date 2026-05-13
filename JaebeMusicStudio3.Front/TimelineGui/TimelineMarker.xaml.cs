using System.Windows.Controls;

namespace JaebeMusicStudio3.Front.TimelineGui;

public partial class TimelineMarker : UserControl
{
    public TimelineMarker()
    {
        InitializeComponent();
        Render();
        MouseDown += (s, e) =>
        {
            var pos = e.GetPosition(this);
            var timeInSeconds = OffsetInSeconds + pos.X * SecondsPerPixel;
            TimeClicked?.Invoke(this, timeInSeconds);
        };
    }

    public event EventHandler<double> TimeClicked;

    public double SecondsPerPixel
    {
        get;
        set
        {
            field = value;
            Render();
        }
    } = 0.01;

    public double OffsetInSeconds
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

    private void Render()
    {
        MainGrid.Children.Clear();
        var stepInSeconds = getStep();
        var lengthInSeconds = ActualWidth * SecondsPerPixel;
        for (var i = Math.Floor(OffsetInSeconds / stepInSeconds) * stepInSeconds;
             i <= lengthInSeconds + OffsetInSeconds;
             i += stepInSeconds)
        {
            var label = new Label();
            label.Content = formatByStep(i, stepInSeconds);
            label.Margin = new System.Windows.Thickness((i - OffsetInSeconds) / SecondsPerPixel, 0, 0, 0);
            label.VerticalAlignment = System.Windows.VerticalAlignment.Top;
            label.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            MainGrid.Children.Add(label);
        }
    }

    private double getStep()
    {
        var basePixelsStep = 100;
        if (basePixelsStep * SecondsPerPixel <= 1)
        {
            var stepInSeconds1 = Math.Pow(10, Math.Ceiling(Math.Log10(basePixelsStep * SecondsPerPixel)));
            var stepInSeconds2 = Math.Pow(10, Math.Ceiling(Math.Log10(basePixelsStep * SecondsPerPixel * 2))) / 2;
            var stepInSeconds5 = Math.Pow(10, Math.Ceiling(Math.Log10(basePixelsStep * SecondsPerPixel * 5))) / 5;
            return Math.Min(stepInSeconds1, Math.Min(stepInSeconds2, stepInSeconds5));
        }
        else
        {
            var bases = new double[] { 1, 5, 15, 60, 5 * 60, 15 * 60, 60 * 60, 3 * 60 * 60, 6 * 60 * 60, 24 * 60 * 60 };
            foreach (var b in bases)
            {
                if (b / SecondsPerPixel > basePixelsStep)
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
        if (value >= 3600)
        {
            var hours = Math.Floor(value / 3600);
            ret += hours.ToString().PadLeft(2, '0') + ":";
            value -= hours * 3600;
            var minutes = Math.Floor(value / 60);
            ret += minutes.ToString().PadLeft(2, '0') + ":";
            value -= minutes * 60;
        }
        else if (value >= 60)
        {
            var minutes = Math.Floor(value / 60);
            ret += minutes.ToString().PadLeft(2, '0') + ":";
            value -= minutes * 60;
        }

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