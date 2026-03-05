namespace MathConverter.Demo.Demos;

public partial class CountClicksPage : ContentPage
{
    public CountClicksPage() => InitializeComponent();
    private void Button_Clicked(object sender, EventArgs e) => NumClicks++;

    public int NumClicks
    {
        get => (int)GetValue(NumClicksProperty);
        set => SetValue(NumClicksProperty, value);
    }
    public static readonly BindableProperty NumClicksProperty = BindableProperty.Create(nameof(NumClicks), typeof(int), typeof(CountClicksPage), 0);
}
