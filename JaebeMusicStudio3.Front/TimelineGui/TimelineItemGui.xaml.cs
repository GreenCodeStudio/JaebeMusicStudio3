using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;
using JaebeMusicStudio3.Core.Timeline;


namespace JaebeMusicStudio3.Front.TimelineGui;

public partial class TimelineItemGui : UserControl
{
    public TimelineItemGui(ITimelineItem item, double secondsPerPixel, double offsetRelative, double horizontalLineHeight)
    {
        InitializeComponent();
        if (item is RecordedSound itemC)
        {
            //     var line = new Polyline();
            //     line.Stroke = System.Windows.Media.Brushes.Green;
            //     MainGrid.Children.Add(line);
            //     var halfHeight = 50;
            //     if (itemC.Samples != null)
            //     {
            //         var widthOfSample = 1.0 / itemC.WaveFormat.SampleRate / secondsPerPixel;
            //         var i = 0;
            //         foreach (var sample in itemC.Samples)
            //         {
            //             line.Points.Add(
            //                 new Point(
            //                     i * widthOfSample,
            //                     sample * halfHeight + halfHeight
            //                 )
            //             );
            //             i++;
            //         }
            //     }
        }
        else if (item is NoteLine itemN)
        {
            if (itemN.Notes.Any())
            {
                var grid = new Grid();
                grid.Background = System.Windows.Media.Brushes.LightGray;
                var minPitch = Math.Log2(itemN.Notes.Min(x => x.Pitch)) * 12;
                var maxPitch = Math.Log2(itemN.Notes.Max(x => x.Pitch)) * 12;
                foreach (var n in itemN.Notes)
                {
                    var pitch = Math.Log2(n.Pitch) * 12;
                    var rect = new Rectangle();
                    rect.VerticalAlignment = VerticalAlignment.Top;
                    rect.HorizontalAlignment = HorizontalAlignment.Left;
                    rect.Margin = new Thickness((n.Start / itemN.Tempo * 60 + offsetRelative) / secondsPerPixel,
                        (maxPitch - pitch) / (maxPitch-minPitch + 1) * horizontalLineHeight, 0, 0);
                    rect.Width = n.Length / itemN.Tempo * 60 / secondsPerPixel;
                    rect.Height = 1 / (maxPitch - minPitch + 1) * horizontalLineHeight;
                    rect.Fill = System.Windows.Media.Brushes.Green;
                    rect.Stroke = System.Windows.Media.Brushes.Black;
                    grid.Children.Add(rect);
                }

                MainGrid.Children.Add(grid);
            }
        }
    }
}