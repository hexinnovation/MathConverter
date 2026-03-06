using System.Windows;

namespace MathConverter.Demo.Demos;

public partial class CountClicks : Window
{
    public CountClicks() => InitializeComponent();
    private void Button_Click(object sender, RoutedEventArgs e) => NumClicks++;
    public int NumClicks
    {
        get => (int)GetValue(NumClicksProperty);
        set => SetValue(NumClicksProperty, value);
    }
    public static readonly DependencyProperty NumClicksProperty = DependencyProperty.Register(nameof(NumClicks), typeof(int), typeof(CountClicks), new PropertyMetadata(0));
}
