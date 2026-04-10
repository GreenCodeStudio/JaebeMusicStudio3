using System.Windows;
using System.Windows.Controls;

namespace JaebeMusicStudio3.Front;

public partial class UiWindow : Window
{
    private readonly Func<UserControl> _createMethod;
    private static UiWindow tmp;

    public UiWindow(Func<UserControl> createControl)
    {
        InitializeComponent();
        this._createMethod = createControl;
        var control = createControl();
        this.ContentWrapper.Children.Clear();
        this.ContentWrapper.Children.Add(control);
        this.Title = control.ToString();
        Closed += (s, e) => { System.Windows.Threading.Dispatcher.ExitAllFrames(); };
        DuplicateButton.Click += (s, e) =>
        {
            Open(_createMethod);
        };
    }

    public static void Open(Func<UserControl> createControl)
    {
        Thread thread = new Thread(() =>
        {
            var window = new UiWindow(createControl);
            window.Show();
            System.Windows.Threading.Dispatcher.Run();
        });
        thread.SetApartmentState(ApartmentState.STA);
        thread.Start();
    }
}