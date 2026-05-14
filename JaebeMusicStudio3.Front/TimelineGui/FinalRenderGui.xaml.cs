using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using JaebeMusicStudio3.Core.AudioRendering;
using JaebeMusicStudio3.Core.Mixer;
using Microsoft.Win32;
using Timeline = JaebeMusicStudio3.Core.Timeline.Timeline;

namespace JaebeMusicStudio3.Front.TimelineGui;

public partial class FinalRenderGui : UserControl
{
    private RenderingProcess process;
    private byte[] data;
    private RenderingChunk chunk;

    public FinalRenderGui()
    {
        InitializeComponent();

        Task.Run(()=>Save());
        CompositionTarget.Rendering += (s, e) =>
        {
            if (process != null)
            {
                var total = chunk.Responses.Count;
                var done = chunk.Responses.Values.Count(r => r.IsCompleted);

                progress.Maximum = total;
                progress.Value = done;
            }
            else
            {
                progress.Value = 0;
            }
        };
    }

    private async void Save()
    {
        var renderTask = Render();

        var dialog = new SaveFileDialog()
        {
            Filter = "Wave|*.wav|mp3|*.mp3",
            DefaultExt = "wav",
        };
        if (dialog.ShowDialog() == true)
        {
            await renderTask;
            if (dialog.FileName.EndsWith(".mp3"))
            {
                using (var writer = new NAudio.Lame.LameMP3FileWriter(dialog.FileName, process.WaveFormat,
                           NAudio.Lame.LAMEPreset.VBR_90))
                {
                    writer.Write(data.AsSpan());
                }
            }
            else
            {
                using (var writer = new NAudio.Wave.WaveFileWriter(dialog.FileName, process.WaveFormat))
                {
                    writer.Write(data.AsSpan());
                }
            }
        }
    }

    private Task Render()
    {
        return Task.Run(() =>
        {
            var start = DateTime.Now;
            this.process = new RenderingProcess(true, RenderingProcess.Current.WaveFormat);
            this.chunk = process.GetChunk((long)(Timeline.Current.TotalLength * process.SampleRate));
            AudioMixer.Current.Render(chunk);

            var response = chunk.GetResponse(AudioMixer.Current.MainOutput).Result;
            this.data = new byte[chunk.Length * process.WaveFormat.BitsPerSample / 8];
            if (response is SingleChannelAudioBuffer)
            {
                (response as SingleChannelAudioBuffer).WriteToRaw(process.WaveFormat, data.AsSpan());
            }

            var end = DateTime.Now;
            Dispatcher.Invoke(() => { Status.Content += $"Render finished in {(end - start).TotalSeconds} seconds"; });
        });
    }
}