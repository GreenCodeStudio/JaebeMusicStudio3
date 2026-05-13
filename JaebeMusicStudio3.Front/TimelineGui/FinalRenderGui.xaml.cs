using System.Windows.Controls;
using System.Windows.Media.Animation;
using JaebeMusicStudio3.Core.AudioRendering;
using JaebeMusicStudio3.Core.Mixer;
using Microsoft.Win32;
using Timeline = JaebeMusicStudio3.Core.Timeline.Timeline;

namespace JaebeMusicStudio3.Front.TimelineGui;

public partial class FinalRenderGui : UserControl
{
    public FinalRenderGui()
    {
        InitializeComponent();
        Task.Run(() =>
        {
            var start = DateTime.Now;
            var process = new RenderingProcess(true, RenderingProcess.Current.WaveFormat);
            var chunk = process.GetChunk((long)(Timeline.Current.TotalLength * process.SampleRate));
            AudioMixer.Current.Render(chunk);

            var response = chunk.GetResponse(AudioMixer.Current.MainOutput).Result;
            var span = new Span<byte>(new byte[chunk.Length * process.WaveFormat.BitsPerSample / 8]);
            if (response is SingleChannelAudioBuffer)
            {
                (response as SingleChannelAudioBuffer).WriteToRaw(process.WaveFormat, span);
            }

            var end = DateTime.Now;
            Dispatcher.Invoke(() => { Status.Content = $"Render finished in {(end - start).TotalSeconds} seconds"; });
            var dialog = new SaveFileDialog()
            {
                Filter = "Wave|*.wav|mp3|*.mp3",
                DefaultExt = "wav",
            };
            if (dialog.ShowDialog() == true)
            {
                if (dialog.FileName.EndsWith(".mp3"))
                {
                    using (var writer = new NAudio.Lame.LameMP3FileWriter(dialog.FileName, process.WaveFormat,
                               NAudio.Lame.LAMEPreset.VBR_90))
                    {
                        writer.Write(span);
                    }
                }
                else
                {
                    using (var writer = new NAudio.Wave.WaveFileWriter(dialog.FileName, process.WaveFormat))
                    {
                        writer.Write(span);
                    }
                }
            }
        });
    }
}