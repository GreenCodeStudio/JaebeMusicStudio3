using System.Runtime.InteropServices;
using NAudio.CoreAudioApi;
using NAudio.Wave;

namespace JaebeMusicStudio3.Core.Timeline;

public class AudioRecorder
{
    private WasapiCapture _capture;
    public RecordedSoundInProgress RecordedSoundInProgress { get; private set; } = new RecordedSoundInProgress();
    public MMDevice Device { get; set; }
    private List<float[]> buffers = new();

    public void StartRecording()
    {
        this._capture = new WasapiCapture();
        _capture.WaveFormat = new WaveFormat(48000, 1);
        _capture.DataAvailable += OnCaptureOnDataAvailable;
        _capture.StartRecording();
        RecordedSoundInProgress.WaveFormat = _capture.WaveFormat;
    }


    private void OnCaptureOnDataAvailable(object? sender, WaveInEventArgs args)
    {
        lock (this)
        {
            //need to copy data here
            var span1 = new Span<byte>(args.Buffer, 0, args.BytesRecorded);
            var span = MemoryMarshal.Cast<byte, short>(span1);
            var copied = new float[span.Length];
            for (var i = 0; i < span.Length; i++)
            {
                copied[i] = (float)span[i] / (float)0x7fff;
            }

            buffers.Add(copied);
        }
    }

    public void StopRecording()
    {
        _capture.StopRecording();
        RecordedSoundInProgress.Samples = buffers.SelectMany(x => x).ToArray();
    }
}