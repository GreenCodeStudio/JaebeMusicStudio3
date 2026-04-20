using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace JaebeMusicStudio3.Front.Controls;

public partial class LabeledControl : UserControl, IAddChild
{
    public string Title
    {
        get;
        set
        {
            Label.Content = value;
            field = value;
        }
    }

    public UIElement Content
    {
        get;
        set
        {
            Child.Children.Clear();
            Child.Children.Add(value);
            field = value;
        }
    }

    public LabeledControl()
    {
        InitializeComponent();
    }
}