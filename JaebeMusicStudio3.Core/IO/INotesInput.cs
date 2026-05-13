using JaebeMusicStudio3.Core.AudioNodes;
using JaebeMusicStudio3.Core.AudioRendering;
using JaebeMusicStudio3.Core.Timeline;

namespace JaebeMusicStudio3.Core.IO;

public interface INotesInput
{
    public string Id { get; }
    public Instrument Instrument { get; set; }
    List<Note> GetNotes(RenderingChunk chunk);
}