using System.Windows;
using System.Windows.Controls;

namespace JaebeMusicStudio3.Front.Controls;

public partial class Number : UserControl
{
    public SliderStyle sliderStyle { get; set; } = SliderStyle.Linear;
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
        Slider.Value = EncodeSlider(Value);
        Slider.Minimum = EncodeSlider(Min);
        Slider.Maximum = EncodeSlider(Max);
    }

    public event Action<double> ValueChanged;

    private void Slider_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        Value = DecodeSlider(Slider.Value);
    }

    private void Box_OnTextChanged(object sender, TextChangedEventArgs e)
    {
        var number = 0.0;
        if(double.TryParse(Box.Text, out number))
        {
            Value = number;
        }
    }

    private double EncodeSlider(double value)
    {
        if(sliderStyle == SliderStyle.Linear)
        {
            return value;
        }
        else if(sliderStyle == SliderStyle.Logarithmic)
        {
            return Math.Log10(value);
        }
        else
        {
            throw new NotImplementedException();
        }
    }

    private double DecodeSlider(double value)
    {
        if(sliderStyle == SliderStyle.Linear)
        {
            return value;
        }
        else if(sliderStyle == SliderStyle.Logarithmic)
        {
            return Math.Pow(10, value);
        }
        else
        {
            throw new NotImplementedException();
        }
    }
}