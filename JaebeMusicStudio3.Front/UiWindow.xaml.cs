using System.Windows;
using System.Windows.Controls;

namespace JaebeMusicStudio3.Front;

public partial class UiWindow : Window
{
    private static UiWindow tmp;

    public UiWindow(Func<UserControl> createControl)
    {
        InitializeComponent();
        var control = createControl();
        this.Content = control;
        this.Title = control.ToString();
        Closed += (s, e) => { System.Windows.Threading.Dispatcher.ExitAllFrames(); };
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