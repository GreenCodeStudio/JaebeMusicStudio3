using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using JaebeMusicStudio3.Core.AudioNodes;
using JaebeMusicStudio3.Core.Mixer;

namespace JaebeMusicStudio3.Front.MixerGui;

public partial class InstrumentEditorGui : UserControl
{
    private double Scale = 1.0;
    private double X = 0.0;
    private double Y = 0.0;

    public InstrumentEditorGui(Instrument instrument)
    {
        this.Instrument = instrument;
        InitializeComponent();
        Render();
        instrument.Changed += () => { Dispatcher.Invoke(() => Render()); };
        GenerateContextMenu();

        this.MouseMove += (sender, args) =>
        {
            if (args.LeftButton == System.Windows.Input.MouseButtonState.Pressed)
            {
                if (_movingNode != null)
                {
                    var pos = Instrument.GetPosition(_movingNode);
                    pos.X += (args.MouseDevice.GetPosition(Plane).X - _movingPoint.Value.X) / Scale;
                    pos.Y += (args.MouseDevice.GetPosition(Plane).Y - _movingPoint.Value.Y) / Scale;
                    _movingPoint = args.MouseDevice.GetPosition(Plane);
                    Instrument.SetPosition(_movingNode, pos);
                }
            }
            else if (args.MiddleButton == System.Windows.Input.MouseButtonState.Pressed)
            {
                X += (args.MouseDevice.GetPosition(Plane).X - _movingPoint.Value.X) / Scale;
                Y += (args.MouseDevice.GetPosition(Plane).Y - _movingPoint.Value.Y) / Scale;
                _movingPoint = args.MouseDevice.GetPosition(Plane);
                Render();
            }
        };
        this.MouseUp += (sender, args) =>
        {
            _movingNode = null;
            _movingPoint = null;
        };
        MouseWheel += (sender, args) =>
        {
            var scaleValue = Math.Pow(2, args.Delta / 200.0);
            var mousePos = args.MouseDevice.GetPosition(PlaneWrapper);
            var scaleBefore = Scale;
            var scaleAfter = Scale * scaleValue;
            X = mousePos.X / scaleAfter - mousePos.X / scaleBefore + X;
            Y = mousePos.Y / scaleAfter - mousePos.Y / scaleBefore + Y;
            Scale = scaleAfter;
            Render();
        };
        MouseDown += (sender, args) =>
        {
            _movingPoint = args.MouseDevice.GetPosition(Plane);
            if (!args.Handled)
            {
                _selectedNode = null;
                PropertiesWrapper.Visibility = Visibility.Collapsed;
                Render();
            }
        };
    }

    private void GenerateContextMenu()
    {
        var nodes = new (string, Func<IAudioNode>)[]
        {
            ("Mix", () => new MixNode()),
            ("Basic oscilator", () => new BasicOscillatorNode()),
            ("Overdrive", () => new OverdriveNode()),
            ("Note modification", () => new NoteModificationNode()),
            ("Note gate", () => new NoteGateNode()),
            ("Noise", () => new Noise()),
        };
        var contextMenu = new ContextMenu();
        this.ContextMenu = contextMenu;
        var add = new MenuItem();
        add.Header = "Add";
        contextMenu.Items.Add(add);

        foreach (var node in nodes)
        {
            var item = new MenuItem();
            item.Header = node.Item1;
            item.Click += (s, e) =>
            {
                var n = node.Item2();
                Instrument.Add(n);
            };
            add.Items.Add(item);
        }
    }

    public Instrument Instrument { get; set; }
    private IAudioNode _movingNode = null;
    private Point? _movingPoint = null;
    private IAudioNode _selectedNode;

    private void Render()
    {
        Plane.Children.Clear();
        var map = new Dictionary<IAudioNode, UserControl>();
        var y = 10;
        foreach (var node in Instrument.Nodes)
        {
            var nodeGui = new MixerNodeGui(node);
            nodeGui.Width = 200;
            nodeGui.Height = 100;

            var pos = Instrument.GetPosition(node);
            nodeGui.Margin = new System.Windows.Thickness((pos.X + X) * Scale, (pos.Y + Y) * Scale, 0, 0);
            if (node == _selectedNode)
            {
                nodeGui.BorderBrush = System.Windows.Media.Brushes.Red;
                nodeGui.BorderThickness = new System.Windows.Thickness(2);
                nodeGui.Margin = new System.Windows.Thickness((pos.X - 2 + X) * Scale, (pos.Y - 2 + Y) * Scale, 0, 0);
                nodeGui.Width += 4;
                nodeGui.Height += 4;
            }

            nodeGui.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            nodeGui.VerticalAlignment = System.Windows.VerticalAlignment.Top;
            Plane.Children.Add(nodeGui);
            map.Add(node, nodeGui);
            y += 100;
            nodeGui.MouseDown += (s, e) =>
            {
                e.Handled = true;
                _selectedNode = node;
                PropertiesWrapper.Visibility = Visibility.Visible;
                CustomPropertiesWrapper.Children.Clear();
                PropertyName.Text = node.Name;
                var x = GuiFactory.Create(node);
                if (x != null)
                {
                    CustomPropertiesWrapper.Children.Add(x);
                }

                _movingNode = node;
                _movingPoint = e.MouseDevice.GetPosition(Plane);
                Render();
            };
            nodeGui.ContextMenu = new ContextMenu();

            nodeGui.MouseUp += (sender, args) =>
            {
                _movingNode = null;
                _movingPoint = null;
            };
            nodeGui.Connect += (input, output) => { Instrument.Connect(output, input); };
            nodeGui.DisconnectByOutput += (output) =>
            {
                foreach (var keyValuePair in Instrument.Connections.Where(x => x.Value == output))
                {
                    Instrument.Disconnect(keyValuePair);
                }
            };
            nodeGui.DisconnectByInput += (input) =>
            {
                foreach (var keyValuePair in Instrument.Connections.Where(x => x.Key == input))
                {
                    Instrument.Disconnect(keyValuePair);
                }
            };
            nodeGui.RenderTransform = new ScaleTransform(Scale, Scale);
        }

        foreach (var x in Instrument.Connections)
        {
            var lineStartNode = map[x.Key.Node];
            var lineEndNode = map[x.Value.Node];
            var line = new Line();
            var startIndex = x.Key.Node.Inputs.Select(n => n.Name).ToArray().IndexOf(x.Key.Name);
            var endIndex = x.Value.Node.Outputs.Select(n => n.Name).ToArray().IndexOf(x.Value.Name);
            line.Stroke = System.Windows.Media.Brushes.Black;
            line.X1 = lineStartNode.Margin.Left;
            line.Y1 = lineStartNode.Margin.Top + (40 + 30 * startIndex) * Scale;
            line.X2 = lineEndNode.Margin.Left + lineEndNode.Width * Scale;
            line.Y2 = lineEndNode.Margin.Top + (40 + 30 * endIndex) * Scale;
            Plane.Children.Add(line);
        }
    }

    private void PropertyNameChanged(object sender, TextChangedEventArgs e)
    {
        _selectedNode.Name = PropertyName.Text;
    }
}