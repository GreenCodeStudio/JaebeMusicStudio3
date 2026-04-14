using System.Windows.Controls;

namespace JaebeMusicStudio3.Front.TimelineGui;

public partial class TimelineMarker : UserControl
{
    public TimelineMarker()
    {
        InitializeComponent();
        Render();
    }

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
        var stepInSeconds = 1;
        var lengthInSeconds = ActualWidth * SecondsPerPixel;
        for (var i = Math.Floor(OffsetInSeconds / stepInSeconds) * stepInSeconds;
             i <= lengthInSeconds + OffsetInSeconds;
             i += stepInSeconds)
        {
            var label = new Label();
            label.Content = i.ToString();
            label.Margin = new System.Windows.Thickness((i - OffsetInSeconds) / SecondsPerPixel, 0, 0, 0);
            label.VerticalAlignment = System.Windows.VerticalAlignment.Top;
            label.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            MainGrid.Children.Add(label);
        }
    }
}