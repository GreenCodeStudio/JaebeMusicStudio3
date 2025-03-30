using System.Runtime.InteropServices;
using JaebeMusicStudio3.Core.AudioRendering;
using NAudio.CoreAudioApi;
using NAudio.Wave;

namespace JaebeMusicStudio3.Core.AudioNodes;

public class LiveAudioInput : IAudioNode
{
    private readonly int DeviceID;
    private List<byte[]> buffers = new();
    private int bufferPosition = 0;
    public IEnumerable<NodeInputDefinition> Inputs => new List<NodeInputDefinition>();

    public IEnumerable<NodeOutputDefinition> Outputs => new List<NodeOutputDefinition>()
    {
        new NodeOutputDefinition()
        {
            Name = "main",
            Node = this,
            Type = NodeConnectionType.SingleChannelAudio
        }
    };

    public Task<Dictionary<string, object>> Render(RenderingChunk chunk)
    {
        var buffor = new SingleChannelAudioBuffer(chunk.Process.SampleRate, chunk.Length);
        var i = 0;
        lock (this)
        {
            while (this.buffers.Any())
            {
                var span = MemoryMarshal.Cast<byte, short>(new Span<byte>(this.buffers.First(), 0,
                    this.buffers.First().Length));
                while (bufferPosition < span.Length && i < buffor.Data.Length)
                {
                    buffor.Data[i] = span[bufferPosition] / (float)0x7fff;
                    i++;
                    bufferPosition++;
                }

                if (bufferPosition >= span.Length)
                {
                    bufferPosition = 0;
                    buffers.RemoveAt(0);
                }

                if (i >= buffor.Data.Length)
                {
                    break;
                }
            }
        }

        return Task.FromResult(new Dictionary<string, object>() { { "main", buffor } });
    }

    public LiveAudioInput(int deviceID)
    {
        this.DeviceID = deviceID;
        var capture = new WasapiCapture();
        capture.WaveFormat = new WaveFormat(48000, 1);
        capture.DataAvailable += (sender, args) =>
        {
            lock (this)
            {
//need to copy data
                buffers.Add(new Span<byte>(args.Buffer, 0, args.BytesRecorded).ToArray());
            }
        };
        capture.StartRecording();
    }
}