using System.Windows;
using System.Windows.Controls;

namespace JaebeMusicStudio3.Front.Controls;

public class LabeledControl2:ContentControl
{
    private readonly StackPanel wrapper;
    private readonly Label label;

    public string Title
    {
        get;
        set
        {
            label.Content = value;
            field = value;
        }
    }

    public LabeledControl2()
    {
        this.wrapper = new StackPanel();
        base.Content = this.wrapper;
        wrapper.Background = System.Windows.Media.Brushes.LightGray;
        this.label = new Label();
        label.Content = Title;
        wrapper.Children.Add(label);
        wrapper.Background = System.Windows.Media.Brushes.White;
        
    }

    public UIElement Content
    {
        get;
        set
        {
            wrapper.Children.Add(value);
            field = value;
        }
    }
}