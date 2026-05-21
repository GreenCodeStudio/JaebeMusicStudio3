using System.Windows.Controls;
using JaebeMusicStudio3.Core.AudioRendering;
using JaebeMusicStudio3.Core.Timeline;
using JaebeMusicStudio3.Front.TimelineGui.NoteLineEditor;

namespace JaebeMusicStudio3.Front.TimelineGui;

public partial class TimelineNotesView : UserControl
{
    private readonly HorizontalNoteEditor NoteEditor;

    public TimelineNotesView(Timeline timeline)
    {
        InitializeComponent();

        this.NoteEditor = new HorizontalNoteEditor(() => timeline.Items.SelectMany(n =>
            (n as NoteLine)?.Notes.Select(x => new Note()
            {
                Length = x.Length / (n as NoteLine).Tempo * 60,
                Start = x.Start / (n as NoteLine).Tempo * 60 + (n as NoteLine).OffsetSeconds,
                Pitch = x.Pitch,
                Volume = x.Volume,
            })), () => 0, false, () =>
        {
            if (RenderingProcess.Current.UseTimeline)
            {
                return new double[]
                    { ((double)RenderingProcess.Current.Position / RenderingProcess.Current.SampleRate )};
            }
            else
            {
                return new double[0];
            }
        });
        MainGrid.Children.Add(NoteEditor);
        timeline.Changed += () =>
        {
            Dispatcher.Invoke(() => { NoteEditor.Render(); });
            refreshEventListeners(timeline);
        };
        refreshEventListeners(timeline);
    }

    private void refreshEventListeners(Timeline timeline)
    {
        foreach (var timelineItem in timeline.Items)
        {
            timelineItem.Changed -= TimelineChanged;
            timelineItem.Changed += TimelineChanged;
        }
    }

    private void TimelineChanged()
    {
        Dispatcher.Invoke(() => { NoteEditor.Render(); });
    }
}