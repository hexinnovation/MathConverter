namespace MathConverter.Demo.Demos;

public partial class NowPlusSixHoursPage : ContentPage
{
    public NowPlusSixHoursPage()
    {
        InitializeComponent();

        var timer = new Timer(Tick, null, 1000, 1000);

        Unloaded += (_, _) => timer.Dispose();
    }
    private void Tick(object? _) => Dispatcher.Dispatch(() => OneSecondTimer = !OneSecondTimer);

    public bool OneSecondTimer
    {
        get => (bool)GetValue(OneSecondTimerProperty);
        set => SetValue(OneSecondTimerProperty, value);
    }
    public static readonly BindableProperty OneSecondTimerProperty = BindableProperty.Create(nameof(OneSecondTimer), typeof(bool), typeof(NowPlusSixHoursPage), false);
}
