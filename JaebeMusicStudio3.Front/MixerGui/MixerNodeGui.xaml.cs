using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Shapes;
using JaebeMusicStudio3.Core.AudioNodes;
using JaebeMusicStudio3.Core.Mixer;

namespace JaebeMusicStudio3.Front.MixerGui;

public partial class MixerNodeGui : UserControl
{
    public event Action<NodeInputDefinition, NodeOutputDefinition> Connect;
    public event Action< NodeOutputDefinition> DisconnectByOutput;
    public event Action<NodeInputDefinition> DisconnectByInput;

    public MixerNodeGui(IAudioNode node)
    {
        InitializeComponent();
        Title.Content = node.Title;
        foreach (var nodeInputDefinition in node.Inputs)
        {
            var item = new Label();
            item.Height = 30;
            item.Content = nodeInputDefinition.Name;
            item.AllowDrop = true;
            item.MouseDown += (x, y) =>
            {
                if(y.LeftButton == MouseButtonState.Pressed)
                System.Windows.DragDrop.DoDragDrop(item, nodeInputDefinition, System.Windows.DragDropEffects.Move);
            };

            item.Drop += (x, y) =>
            {
                if (y.Data.GetDataPresent(typeof(NodeOutputDefinition)))
                {
                    y.Effects = System.Windows.DragDropEffects.Move;
                    Connect?.Invoke(nodeInputDefinition,
                        (NodeOutputDefinition)y.Data.GetData(typeof(NodeOutputDefinition)));
                }
            };
            var contextMenu = new ContextMenu();
            var disconnect = new MenuItem();
            disconnect.Header = "Disconnect";
            disconnect.Click += (x, y) =>
            {
                DisconnectByInput?.Invoke(nodeInputDefinition);
            };
            contextMenu.Items.Add(disconnect);
            item.ContextMenu = contextMenu;
            this.Inputs.Children.Add(item);
        }

        foreach (var nodeOutputDefinition in node.Outputs)
        {
            var item = new Grid();
            var label = new Label();
            item.Children.Add(label);
            item.Height = 30;
            label.Content = nodeOutputDefinition.Name;
            item.AllowDrop = true;
            item.MouseDown += (x, y) =>
            {
                if(y.LeftButton == MouseButtonState.Pressed)
                System.Windows.DragDrop.DoDragDrop(item, nodeOutputDefinition, System.Windows.DragDropEffects.Move);
            };
            item.Drop += (x, y) =>
            {
                if (y.Data.GetDataPresent(typeof(NodeInputDefinition)))
                {
                    y.Effects = System.Windows.DragDropEffects.Move;
                    Connect?.Invoke((NodeInputDefinition)y.Data.GetData(typeof(NodeInputDefinition)),
                        nodeOutputDefinition);
                }
            };
            this.Outputs.Children.Add(item);

            var contextMenu = new ContextMenu();
            var soundAnalizer = new MenuItem();
            soundAnalizer.Header = "Sound Analizer";
            soundAnalizer.Click += (x, y) => { UiWindow.Open(() => new SoundAnalyzer(nodeOutputDefinition)); };
            contextMenu.Items.Add(soundAnalizer);
            var disconnect = new MenuItem();
            disconnect.Header = "Disconnect";
            disconnect.Click += (x, y) =>
            {
                DisconnectByOutput?.Invoke(nodeOutputDefinition);
            };
            contextMenu.Items.Add(disconnect);
            item.ContextMenu = contextMenu;
            
            var nanoSoundIndicator=new NanoSoundAnalyzer(nodeOutputDefinition);
            nanoSoundIndicator.HorizontalAlignment = HorizontalAlignment.Right;
            nanoSoundIndicator.VerticalAlignment = VerticalAlignment.Center;
            item.Children.Add(nanoSoundIndicator);
        }
    }
}