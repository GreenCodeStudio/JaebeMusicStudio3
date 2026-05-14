using System.Runtime.CompilerServices;
using System.Windows.Input;
using JaebeMusicStudio3.Core.AudioNodes;
using JaebeMusicStudio3.Core.AudioRendering;
using JaebeMusicStudio3.Core.Timeline;

namespace JaebeMusicStudio3.Core.IO;

public class KeyboardInput : INotesInput
{
    public static KeyboardInput singleton1 = new KeyboardInput(Type.lower);
    public static KeyboardInput singleton2 = new KeyboardInput(Type.upper);

    private Dictionary<RenderingProcess, Dictionary<System.Windows.Input.Key, Note>>
        pressedNotes = new();

    private Dictionary<RenderingProcess, List<Note>> oldNotes = new();

    private readonly Type type;
    private readonly Thread thread;

    public string Id => $"KeyboardInput{type}";
    public Instrument Instrument { get; set; }

    public List<Note> GetNotes(RenderingChunk chunk)
    {
        return System.Windows.Threading.Dispatcher.FromThread(thread).Invoke(() =>
        {
          
                if (!pressedNotes.ContainsKey(chunk.Process))
                {
                    pressedNotes[chunk.Process] = new();
                }

                if (!oldNotes.ContainsKey(chunk.Process))
                {
                    oldNotes[chunk.Process] = new();
                }

                var currentPressedNotes = pressedNotes[chunk.Process];
                var currentOldNotes = oldNotes[chunk.Process];
                foreach (var x in GetKeys())
                {
                    if (Keyboard.IsKeyDown(x.key))
                    {
                        if (!currentPressedNotes.ContainsKey(x.key))
                        {
                            var newNote = new Note()
                            {
                                Start = (float)chunk.Start / chunk.Process.SampleRate, Length = float.MaxValue,
                                Pitch = x.pitch
                            };
                            currentPressedNotes.Add(x.key, newNote);
                        }
                    }
                    else
                    {
                        if (currentPressedNotes.ContainsKey(x.key))
                        {
                            var endingNote = currentPressedNotes[x.key];
                            currentPressedNotes.Remove(x.key);
                            endingNote.Length = (float)(chunk.Start / chunk.Process.SampleRate - endingNote.Start);
                            currentOldNotes.Add(endingNote);
                        }
                    }
                }

                return currentOldNotes.Concat(currentPressedNotes.Values).ToList();
            
        });
    }

    public KeyboardInput(Type type)
    {
        this.type = type;
        this.thread = new Thread(() => { System.Windows.Threading.Dispatcher.Run(); });
        thread.Name = "KeyboardInputThread";
        thread.SetApartmentState(ApartmentState.STA);
        thread.Start();
    }


    public enum Type
    {
        lower,
        upper
    }

    private IEnumerable<(Key key, float pitch)> GetKeys()
    {
        if (type == Type.lower)
        {
            yield return (Key.Z, 440 * MathF.Pow(2, -9f / 12));
            yield return (Key.S, 440 * MathF.Pow(2, -8f / 12));
            yield return (Key.X, 440 * MathF.Pow(2, -7f / 12));
            yield return (Key.D, 440 * MathF.Pow(2, -6f / 12));
            yield return (Key.C, 440 * MathF.Pow(2, -5f / 12));
            yield return (Key.V, 440 * MathF.Pow(2, -4f / 12));
            yield return (Key.G, 440 * MathF.Pow(2, -3f / 12));
            yield return (Key.B, 440 * MathF.Pow(2, -2f / 12));
            yield return (Key.H, 440 * MathF.Pow(2, -1f / 12));
            yield return (Key.N, 440);
            yield return (Key.J, 440 * MathF.Pow(2, 1f / 12));
            yield return (Key.M, 440 * MathF.Pow(2, 2f / 12));
            yield return (Key.OemComma, 440 * MathF.Pow(2, 3f / 12));
            yield return (Key.L, 440 * MathF.Pow(2, 4f / 12));
            yield return (Key.OemPeriod, 440 * MathF.Pow(2, 5f / 12));
            yield return (Key.Oem1, 440 * MathF.Pow(2, 6f / 12));
            yield return (Key.OemQuestion, 440 * MathF.Pow(2, 7f / 12));
            yield return (Key.Oem2, 440 * MathF.Pow(2, 8f / 12));
            yield return (Key.Oem7, 440 * MathF.Pow(2, 9f / 12));
            yield return (Key.RightShift, 440 * MathF.Pow(2, 10f / 12));
            yield return (Key.Enter, 440 * MathF.Pow(2, 11f / 12));
        }
        else
        {
            yield return (Key.Q, 440 * MathF.Pow(2, 3f / 12));
            yield return (Key.D2, 440 * MathF.Pow(2, 4f / 12));
            yield return (Key.W, 440 * MathF.Pow(2, 5f / 12));
            yield return (Key.D3, 440 * MathF.Pow(2, 6f / 12));
            yield return (Key.E, 440 * MathF.Pow(2, 7f / 12));
            yield return (Key.R, 440 * MathF.Pow(2, 8f / 12));
            yield return (Key.D5, 440 * MathF.Pow(2, 9f / 12));
            yield return (Key.T, 440 * MathF.Pow(2, 10f / 12));
            yield return (Key.D6, 440 * MathF.Pow(2, 11f / 12));
            yield return (Key.Y, 440 * MathF.Pow(2, 12f / 12));
            yield return (Key.D7, 440 * MathF.Pow(2, 13f / 12));
            yield return (Key.U, 440 * MathF.Pow(2, 14f / 12));
            yield return (Key.I, 440 * MathF.Pow(2, 15f / 12));
            yield return (Key.D9, 440 * MathF.Pow(2, 16f / 12));
            yield return (Key.O, 440 * MathF.Pow(2, 17f / 12));
            yield return (Key.D0, 440 * MathF.Pow(2, 18f / 12));
            yield return (Key.P, 440 * MathF.Pow(2, 19f / 12));
            yield return (Key.OemOpenBrackets, 440 * MathF.Pow(2, 20f / 12));
            yield return (Key.Oem6, 440 * MathF.Pow(2, 21f / 12));
            yield return (Key.OemQuotes, 440 * MathF.Pow(2, 23f / 12));
        }
    }
}