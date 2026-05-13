using System.Windows.Controls;
using JaebeMusicStudio3.Core.IO;

namespace JaebeMusicStudio3.Front.IO;

public partial class IOGui : UserControl
{
    public IOGui()
    {
        InitializeComponent();
        Render();
        IOWrapper.Changed += Render;
    }

    private void Render()
    {
        InputStack.Children.Clear();
        foreach (var x in IOWrapper.NotesInputs)
        {
            InputStack.Children.Add(new InputGui(x));
        }
    }
}