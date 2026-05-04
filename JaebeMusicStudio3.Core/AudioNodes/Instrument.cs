using JaebeMusicStudio3.Core.AudioRendering;
using JaebeMusicStudio3.Core.Timeline;

namespace JaebeMusicStudio3.Core.AudioNodes;

public class Instrument : IAudioNode
{
    public IEnumerable<NodeInputDefinition> Inputs => new List<NodeInputDefinition>()
    {
    };

    public IEnumerable<NodeOutputDefinition> Outputs => new List<NodeOutputDefinition>()
    {
        new NodeOutputDefinition()
        {
            Name = "output",
            Node = this,
            Type = NodeConnectionType.SingleChannelAudio
        }
    };

    public Task<Dictionary<string, object>> Render(RenderingChunk chunk, Dictionary<string, object> inputs)
    {
        var output = new SingleChannelAudioBuffer(chunk.Process.SampleRate, chunk.Length);
        if (chunk.Process.UseTimeline)
        {
            List<NoteLine> noteLines;
            lock (Timeline.Timeline.Current)
            {
                noteLines = Timeline.Timeline.Current.Items.OfType<NoteLine>().Where(l => l.Instrument == this)
                    .ToList();
            }

            var notes = noteLines.SelectMany(x => x.Notes.Select(y => new
            {
                Pitch = y.Pitch,
                Start = y.Start * 60 / x.Tempo + x.OffsetSeconds,
                Length = y.Length * 60 / x.Tempo,
            })).ToList();
            var chunkStartSeconds = chunk.Start / (double)chunk.Process.SampleRate;
            var chunkLengthSeconds = chunk.Length / (double)chunk.Process.SampleRate;
            foreach (var note in notes)
            {
                var startOffset = note.Start - chunkStartSeconds;
                var endOffset = note.Start + note.Length - chunkStartSeconds;
                if (startOffset < chunkLengthSeconds && endOffset > 0)
                {
                    var i = startOffset < 0 ? 0 : (int)(startOffset * chunk.Process.SampleRate);
                    var length = (int)(chunkLengthSeconds * chunk.Process.SampleRate);
                    if (length > chunk.Length)
                        length = (int)chunk.Length;
                    for (; i < length; i++)
                    {
                        var secondsOffset = (float)i / chunk.Process.SampleRate - (float)startOffset;
                        output.Data[i] += MathF.Sin(secondsOffset * (float)note.Pitch * 2 * MathF.PI) * 0.1f;
                    }
                }
            }
        }

        return Task.FromResult(new Dictionary<string, object>() { { "output", output } });
    }

    public string Title => "Instrument";
}