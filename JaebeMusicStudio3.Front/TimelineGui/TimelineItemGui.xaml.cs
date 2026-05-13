using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;
using JaebeMusicStudio3.Core.Timeline;
using JaebeMusicStudio3.Front.Utils;


namespace JaebeMusicStudio3.Front.TimelineGui;

public partial class TimelineItemGui : UserControl
{
    public TimelineItemGui(ITimelineItem item, double secondsPerPixel, double offsetRelative,
        double horizontalLineHeight, double visibleLength)
    {
        InitializeComponent();
        if (item is RecordedSound itemC)
        {
            var line = GraphDrawer.GenerateSamplesGraph(itemC.WaveFormat.SampleRate,
                itemC.Samples.Skip((int)(-offsetRelative * itemC.WaveFormat.SampleRate))
                    .Take((int)(visibleLength * itemC.WaveFormat.SampleRate)), secondsPerPixel, horizontalLineHeight);
            MainGrid.Children.Add(line);
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
            this.MouseDoubleClick += (_, _) =>
            {
                UiWindow.Open(() =>
                    new TabView(() => new NoteLineEditor.NoteLineEditor(itemN)
                    )
                );
            };
            if (itemN.Notes.Any())
            {
                var grid = new Grid();
                grid.Background = System.Windows.Media.Brushes.LightGray;
                if (itemN.Instrument != null)
                {
                    var instrument = new Label();
                    instrument.Content = itemN.Instrument;
                    MainGrid.Children.Add(instrument);
                    instrument.Background = System.Windows.Media.Brushes.LightBlue;
                    instrument.VerticalAlignment = VerticalAlignment.Top;
                    instrument.Height = 30;
                    horizontalLineHeight -= 30;
                    grid.Margin = new Thickness(0, 20, 0, 0);
                }

                var minPitch = Math.Log2(itemN.Notes.Min(x => x.Pitch)) * 12;
                var maxPitch = Math.Log2(itemN.Notes.Max(x => x.Pitch)) * 12;
                if (horizontalLineHeight > 0)
                {
                    foreach (var n in itemN.Notes)
                    {
                        var pitch = Math.Log2(n.Pitch) * 12;
                        var rect = new Rectangle();
                        rect.VerticalAlignment = VerticalAlignment.Top;
                        rect.HorizontalAlignment = HorizontalAlignment.Left;
                        rect.Margin = new Thickness((n.Start / itemN.Tempo * 60 + offsetRelative) / secondsPerPixel,
                            (maxPitch - pitch) / (maxPitch - minPitch + 1) * horizontalLineHeight, 0, 0);
                        rect.Width = n.Length / itemN.Tempo * 60 / secondsPerPixel;
                        rect.Height = 1 / (maxPitch - minPitch + 1) * horizontalLineHeight;
                        rect.Fill = System.Windows.Media.Brushes.Green;
                        rect.Stroke = System.Windows.Media.Brushes.Black;
                        grid.Children.Add(rect);
                    }
                }

                MainGrid.Children.Add(grid);
            }
        }
    }
}