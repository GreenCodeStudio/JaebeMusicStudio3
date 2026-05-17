using System.Diagnostics;
using System.IO.Compression;
using System.Reflection;

namespace JaebeMusicStudio3.Installer
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Path.Text =
    System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles),
        "JaebeMusicStudio");

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        { 
            var asm = Assembly.GetExecutingAssembly();
            var stream = asm.GetManifestResourceStream("JaebeMusicStudio3.Installer.Files.zip");
            var zip = new ZipArchive(stream);
            var dir = new DirectoryInfo(Path.Text);
            if (!dir.Exists)
            {
                dir.Create();
            }
            zip.ExtractToDirectory(Path.Text);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var tmpDir = System.IO.Path.Combine(System.IO.Path.GetTempPath(), Guid.NewGuid().ToString());
            var asm = Assembly.GetExecutingAssembly();
            var stream = asm.GetManifestResourceStream("JaebeMusicStudio3.Installer.Files.zip");
            var zip = new ZipArchive(stream);
            var dir = new DirectoryInfo(tmpDir);
            if (!dir.Exists)
            {
                dir.Create();
            }
            zip.ExtractToDirectory(tmpDir);
            Process.Start(System.IO.Path.Combine(tmpDir, "JaebeMusicStudio3.Front.exe"));
        }
    }
}
