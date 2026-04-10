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
    public MixerNodeGui(IAudioNode node)
    {
        InitializeComponent();
        Title.Content = node.Title;
        foreach (var nodeInputDefinition in node.Inputs)
        {
            var item = new Ellipse();
            item.Width = 50;
            item.Height = 50;
            item.Fill = System.Windows.Media.Brushes.Black;
            item.AllowDrop = true;
            item.MouseDown += (x, y) =>
            {
                System.Windows.DragDrop.DoDragDrop(item, nodeInputDefinition, System.Windows.DragDropEffects.Move);
            };
            item.GiveFeedback += (x, e) =>
            {
                if (e.Effects.HasFlag(DragDropEffects.Copy))
                {
                    Mouse.SetCursor(Cursors.Cross);
                }
                else if (e.Effects.HasFlag(DragDropEffects.Move))
                {
                    Mouse.SetCursor(Cursors.Pen);
                }
                else
                {
                    Mouse.SetCursor(Cursors.No);
                }

                e.Handled = true;

                Mouse.SetCursor(Cursors.IBeam);
            };
            item.DragEnter += (x, y) =>
            {
                if (y.Data.GetDataPresent(typeof(NodeInputDefinition)))
                {
                    y.Effects = System.Windows.DragDropEffects.Move;
                }
            };
            item.DragOver += (x, y) =>
            {
                if (y.Data.GetDataPresent(typeof(NodeInputDefinition)))
                {
                    y.Effects = System.Windows.DragDropEffects.Move;
                }
            };
            item.Drop += (x, y) =>
            {
                if (y.Data.GetDataPresent(typeof(NodeOutputDefinition)))
                {
                    y.Effects = System.Windows.DragDropEffects.Move;
                    Connect?.Invoke(nodeInputDefinition, (NodeOutputDefinition)y.Data.GetData(typeof(NodeOutputDefinition)));
                }
            };
            this.Inputs.Children.Add(item);
        }
        foreach (var nodeOutputDefinition in node.Outputs)
        {
            var item = new Ellipse();
            item.Width = 50;
            item.Height = 50;
            item.Fill = System.Windows.Media.Brushes.Black;
            item.AllowDrop = true;
            item.MouseDown += (x, y) =>
            {
                System.Windows.DragDrop.DoDragDrop(item, nodeOutputDefinition, System.Windows.DragDropEffects.Move);
            };
            item.GiveFeedback += (x, e) =>
            {
                if (e.Effects.HasFlag(DragDropEffects.Copy))
                {
                    Mouse.SetCursor(Cursors.Cross);
                }
                else if (e.Effects.HasFlag(DragDropEffects.Move))
                {
                    Mouse.SetCursor(Cursors.Pen);
                }
                else
                {
                    Mouse.SetCursor(Cursors.No);
                }

                e.Handled = true;

                Mouse.SetCursor(Cursors.IBeam);
            };
            item.DragEnter += (x, y) =>
            {
                if (y.Data.GetDataPresent(typeof(NodeOutputDefinition)))
                {
                    y.Effects = System.Windows.DragDropEffects.Move;
                }
            };
            item.DragOver += (x, y) =>
            {
                if (y.Data.GetDataPresent(typeof(NodeOutputDefinition)))
                {
                    y.Effects = System.Windows.DragDropEffects.Move;
                }
            };
            item.Drop += (x, y) =>
            {
                if (y.Data.GetDataPresent(typeof(NodeInputDefinition)))
                {
                    y.Effects = System.Windows.DragDropEffects.Move;
                    Connect?.Invoke((NodeInputDefinition)y.Data.GetData(typeof(NodeInputDefinition)), nodeOutputDefinition);
                }
            };
            this.Outputs.Children.Add(item);
        }
    }
}