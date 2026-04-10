using System.Windows;
using System.Windows.Controls;

namespace JaebeMusicStudio3.Front.Controls;

public partial class VolumeSlider : UserControl
{
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

    public VolumeSlider()
    {
        InitializeComponent();
    }

    private void _render()
    {
        Percent.Text = (Value * 100).ToString();
        Decibels.Text = (20 * System.Math.Log10(Value)).ToString();
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