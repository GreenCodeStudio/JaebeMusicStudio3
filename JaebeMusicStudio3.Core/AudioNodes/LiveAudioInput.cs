using System.Runtime.InteropServices;
using JaebeMusicStudio3.Core.AudioRendering;
using NAudio.CoreAudioApi;
using NAudio.Wave;

namespace JaebeMusicStudio3.Core.AudioNodes;

public class LiveAudioInput : IAudioNode
{
    private readonly int DeviceID;
    private List<float[]> buffers = new();
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

    public Task<Dictionary<string, object>> Render(RenderingChunk chunk, Dictionary<string, object> inputs)
    {
        var buffor = new SingleChannelAudioBuffer(chunk.Process.SampleRate, chunk.Length);
        var i = 0;
        lock (this)
        {
            while (this.buffers.Any())
            {
                var inputBuffer = this.buffers.First();
                while (bufferPosition < inputBuffer.Length && i < buffor.Data.Length)
                {
                    buffor.Data[i] = inputBuffer[bufferPosition] ;
                    i++;
                    bufferPosition++;
                }

                if (bufferPosition >= inputBuffer.Length)
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
        };
        capture.StartRecording();
    }
}