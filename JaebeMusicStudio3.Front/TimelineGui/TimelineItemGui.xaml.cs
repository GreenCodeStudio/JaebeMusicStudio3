using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;
using JaebeMusicStudio3.Core.Timeline;


namespace JaebeMusicStudio3.Front.TimelineGui;

public partial class TimelineItemGui : UserControl
{
    public TimelineItemGui(ITimelineItem item, double secondsPerPixel)
    {
        InitializeComponent();
        if (item is RecordedSound itemC)
        {
            var line = new Polyline();
            line.Stroke = System.Windows.Media.Brushes.Green;
            MainGrid.Children.Add(line);
            var halfHeight = 50;
            if (itemC.Samples != null)
            {
                var widthOfSample = 1.0 / itemC.WaveFormat.SampleRate / secondsPerPixel;
                var i = 0;
                foreach (var sample in itemC.Samples)
                {
                    line.Points.Add(
                        new Point(
                            i * widthOfSample,
                            sample * halfHeight + halfHeight
                        )
                    );
                    i++;
                }
            }
        }
    }
}