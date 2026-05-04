using System.Windows.Controls;
using JaebeMusicStudio3.Core.IO;

namespace JaebeMusicStudio3.Front.IO;

public partial class IOGui : UserControl
{
    public IOGui()
    {
        InitializeComponent();
        foreach (var x in IOWrapper.NotesInputs)
        {
            InputStack.Children.Add(new InputGui(x));
        }
    }
}