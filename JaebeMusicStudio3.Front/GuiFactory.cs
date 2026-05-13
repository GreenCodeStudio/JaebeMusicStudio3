using System.Windows.Controls;
using JaebeMusicStudio3.Core.AudioNodes;
using JaebeMusicStudio3.Front.AudioNodesGui;

namespace JaebeMusicStudio3.Front;

public class GuiFactory
{
    public static UserControl Create(object x)
    {
        if (x is VolumeNode)
        {
            return new VolumeNodeGui(x as VolumeNode);
        }
        else if (x is LiveAudioInput)
        {
            return new LiveAudioInputGui(x as LiveAudioInput);
        }
        else if (x is OverdriveNode)
        {
            return new OverdriveNodeGui(x as OverdriveNode);
        }
        else if (x is Instrument)
        {
            return new InstrumentNodeGui(x as Instrument);
        }
        else if (x is NoteModificationNode)
        {
            return new NoteModificationNodeGui(x as NoteModificationNode);
        }
        else
        {
            return null;
        }
    }
}