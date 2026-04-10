using System.Windows;
using System.Windows.Controls;

namespace JaebeMusicStudio3.Front.Controls;

public partial class Number : UserControl
{
    public Number()
    {
        InitializeComponent();
    }
    
    public double Value
    {
        get;
        set
        {
            field = value;
            _render();
            ValueChanged?.Invoke(value);
        }
    } = 1;

    public double Min
    {
        get;
        set
        {
            field = value;
            _render();
        }
    } = 0;

    public double Max
    {
        get;
        set
        {
            field = value;
            _render();
        }
    } = 1;


    private void _render()
    {
        Box.Text = (Value).ToString();
        Slider.Value = Value;
        Slider.Minimum = Min;
        Slider.Maximum = Max;
    }

    public event Action<double> ValueChanged;

    private void Slider_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        Value = e.NewValue;
    }
}