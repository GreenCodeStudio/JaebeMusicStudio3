using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;

namespace JaebeMusicStudio3.Front;

public partial class TabView : UserControl, ITabbableControl
{
    private UserControl? _selectedContent;

    public TabView(params Func<UserControl>[] createControl)
    {
        InitializeComponent();
        foreach (var control in createControl)
        {
            Add(control);
        }
    }

    public void Add(Func<UserControl> createControl)
    {
        foreach (UserControl child in ContentWrapper.Children)
        {
            child.Visibility = Visibility.Collapsed;
        }

        foreach (Grid x in TabsWrapper.Children)
        {
            x.Children.OfType<Rectangle>().First().Opacity = 0;
        }

        var tab = new Grid();
        tab.Margin = new Thickness(4, 4, 4, -1);
        var rectangle = new System.Windows.Shapes.Rectangle();
        rectangle.Fill = System.Windows.Media.Brushes.White;
        rectangle.Stroke = System.Windows.Media.Brushes.Black;
        tab.Children.Add(rectangle);
        var inside = new StackPanel();
        tab.Children.Add(inside);
        inside.Orientation = Orientation.Horizontal;
        var title = new TextBlock();
        inside.Children.Add(title);
        TabsWrapper.Children.Add(tab);
        var content = createControl();
        ContentWrapper.Children.Add(content);
        title.Text = (content as ITabbableControl)?.Title ?? content.ToString();

        var duplicateButton = new Button();
        duplicateButton.Content = "D";
        duplicateButton.Margin = new Thickness(4);
        duplicateButton.Click += (s, e) => { Add(createControl); };
        inside.Children.Add(duplicateButton);

        var newWindowButton = new Button();
        newWindowButton.Content = "N";
        newWindowButton.Margin = new Thickness(4);
        newWindowButton.Click += (s, e) => { UiWindow.Open(() => new TabView(createControl)); };
        inside.Children.Add(newWindowButton);
        
        var closeButton = new Button();
        closeButton.Content = "X";
        closeButton.Margin = new Thickness(4);
        closeButton.Click += (s, e) =>
        {
            TabsWrapper.Children.Remove(tab);
            ContentWrapper.Children.Remove(content);
        };
        inside.Children.Add(closeButton);


        tab.MouseDown += (s, e) =>
        {
            _selectedContent = content;
            foreach (Grid x in TabsWrapper.Children)
            {
                x.Children.OfType<Rectangle>().First().Opacity = 0;
            }

            rectangle.Opacity = 1;
            foreach (UserControl child in ContentWrapper.Children)
            {
                child.Visibility = child == content ? Visibility.Visible : Visibility.Collapsed;
            }

            ChangedMetadata?.Invoke();
        };
        if (content is ITabbableControl tabbable)
        {
            tabbable.ChangedMetadata += () =>
            {
                title.Text = tabbable.Title;
                ChangedMetadata?.Invoke();
            };
        }

        _selectedContent = content;
        ChangedMetadata?.Invoke();
    }

    public string Title => (_selectedContent as ITabbableControl)?.Title ?? _selectedContent?.ToString() ?? "Empty";
    public event Action? ChangedMetadata;
}