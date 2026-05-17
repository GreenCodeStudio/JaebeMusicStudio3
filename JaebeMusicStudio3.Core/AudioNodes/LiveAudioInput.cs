using System.Runtime.InteropServices;
using System.Text.Json.Serialization;
using JaebeMusicStudio3.Core.AudioRendering;
using JaebeMusicStudio3.Core.Utils;
using NAudio.CoreAudioApi;
using NAudio.Wave;

namespace JaebeMusicStudio3.Core.AudioNodes;

public class LiveAudioInput : IAudioNode
{
    
    public Guid Id { get;set; }=Guid.NewGuid();
    private List<float[]> buffers = new();
    private int bufferPosition = 0;
    private WasapiCapture _capture;
    
    [JsonIgnore]
    public IEnumerable<NodeInputDefinition> Inputs => new List<NodeInputDefinition>();
    
    [JsonIgnore]

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
        var buffer = new SingleChannelAudioBuffer(chunk.Process.SampleRate, chunk.Length);
        var i = 0;
        lock (this)
        {
            while (this.buffers.Any())
            {
                var inputBuffer = this.buffers.First();
                while (bufferPosition < inputBuffer.Length && i < buffer.Data.Length)
                {
                    buffer.Data[i] = inputBuffer[bufferPosition];
                    i++;
                    bufferPosition++;
                }

                if (bufferPosition >= inputBuffer.Length)
                {
                    bufferPosition = 0;
                    buffers.RemoveAt(0);
                }

                if (i >= buffer.Data.Length)
                {
                    break;
                }
            }
        }    
        for (; i < chunk.Length; i++)
        {
            if(i%100==0)
                Console.WriteLine(chunk.Start+ i);
            buffer.Data[i] = (float)Math.Sin((chunk.Start + i) / 100.0) * 0.1f;
        }

        return Task.FromResult(new Dictionary<string, object>() { { "main", buffer } });
    }

    public string Title => "Live Audio Input";

    public LiveAudioInput()
    {
        this._capture = new WasapiCapture();
        _capture.WaveFormat = new WaveFormat(48000, 1);
        _capture.DataAvailable += OnCaptureOnDataAvailable;
        _capture.StartRecording();
    }

    private void OnCaptureOnDataAvailable(object? sender, WaveInEventArgs args)
    {
        lock (this)
        {
            var span1 = new Span<byte>(args.Buffer, 0, args.BytesRecorded);
            var copied = BinaryConverter.FromBinary(_capture.WaveFormat, span1);
            buffers.Add(copied);
        }
    }

    public MMDevice Device
    {
        get;
        set
        {
            _capture.StopRecording();
            this._capture = new WasapiCapture(value);
            _capture.WaveFormat = WaveFormat;
            _capture.DataAvailable += OnCaptureOnDataAvailable;
            _capture.StartRecording();
            field = value;
        }
    }

    public WaveFormat WaveFormat
    {
        get;
        set
        {
            _capture.StopRecording();
            this._capture = new WasapiCapture(Device);
            _capture.WaveFormat = value;
            _capture.DataAvailable += OnCaptureOnDataAvailable;
            _capture.StartRecording();
            field = value;
        }
    }=new  WaveFormat(48000, 2);
    
    
    public string Name { get; set; }
}