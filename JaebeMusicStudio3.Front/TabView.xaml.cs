using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;
using JaebeMusicStudio3.Core.Serialization;
using Microsoft.Win32;

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
        tab.Margin = new Thickness(4, 4, 4, -2);
        var rectangle = new System.Windows.Shapes.Rectangle();
        rectangle.Fill = System.Windows.Media.Brushes.White;
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
        title.HorizontalAlignment = HorizontalAlignment.Center;
        title.VerticalAlignment = VerticalAlignment.Center;
title.Margin = new Thickness(4, 0, 4, 4);

        var duplicateButton = new Button();
        duplicateButton.Content = "D";
        duplicateButton.Margin = new Thickness(2,4,2,8);
        duplicateButton.Click += (s, e) => { Add(createControl); };
        inside.Children.Add(duplicateButton);

        var newWindowButton = new Button();
        newWindowButton.Content = "N";
        newWindowButton.Margin = new Thickness(2,4,2,8);
        newWindowButton.Click += (s, e) => { UiWindow.Open(() => new TabView(createControl)); };
        inside.Children.Add(newWindowButton);
        
        var closeButton = new Button();
        closeButton.Content = "X";
        closeButton.Margin = new Thickness(2,4,2,8);
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

    private void ShowMenu(object sender, RoutedEventArgs e)
    {
        var contextMenu = new ContextMenu();
        var save = new MenuItem();
        save.Header = "Save";
        save.Click += (x, y) =>
        {
            var dialog=new SaveFileDialog()            {
                Filter = "Jaebe Music Studio Project|*.jmsp",
                DefaultExt = "jmsp"
            };
            if (dialog.ShowDialog() == true)
            {
                var path = dialog.FileName;
                Project.ReadLoaded().SaveAsFile(path);
            }
        };
        contextMenu.Items.Add(save);
        var open = new MenuItem();
        open.Header = "Open";
        open.Click += (x, y) =>
        {
            var dialog = new OpenFileDialog()
            {
                Filter = "Jaebe Music Studio Project|*.jmsp",
                DefaultExt = "jmsp"
            };
            if (dialog.ShowDialog() == true)
            {
                var path = dialog.FileName;
                Project.ReadFromFile(path).Load();
            }
        };
        contextMenu.Items.Add(open);
        contextMenu.IsOpen = true;
    }
}