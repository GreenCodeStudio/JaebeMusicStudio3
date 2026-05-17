using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace JaebeMusicStudio3.Installer;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        Path.Text =
            System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles),
                "JaebeMusicStudio");
    }

    private void Install(object sender, RoutedEventArgs e)
    {
        var uri = new Uri("pack://application:,,,/Files.zip");
        var stream = Application.GetResourceStream(uri).Stream;
        var zip = new ZipArchive(stream);
        var dir = new DirectoryInfo(Path.Text);
        if (!dir.Exists)
        {
            dir.Create();
        }
        zip.ExtractToDirectory(Path.Text);
    }

    private void Run(object sender, RoutedEventArgs e)
    {
        var tmpDir=System.IO.Path.Combine( System.IO.Path.GetTempPath(),Guid.NewGuid().ToString());
        var uri = new Uri("pack://application:,,,/Files.zip");
        var stream = Application.GetResourceStream(uri).Stream;
        var zip = new ZipArchive(stream);
        var dir = new DirectoryInfo(tmpDir);
        if (!dir.Exists)
        {
            dir.Create();
        }
        zip.ExtractToDirectory(tmpDir);
        Process.Start(System.IO.Path.Combine(tmpDir,"JaebeMusicStudio3.Front.exe"));
    }
}