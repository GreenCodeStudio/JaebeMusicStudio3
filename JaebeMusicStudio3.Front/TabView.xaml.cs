using System.Windows;
using System.Windows.Controls;

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

        var tab = new Grid();
        tab.Background = System.Windows.Media.Brushes.Aqua;
        var title = new TextBlock();
        tab.Children.Add(title);
        TabsWrapper.Children.Add(tab);
        var content = createControl();
        ContentWrapper.Children.Add(content);
        title.Text = (content as ITabbableControl)?.Title ?? content.ToString();
        tab.MouseDown += (s, e) =>
        {
            _selectedContent = content;
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