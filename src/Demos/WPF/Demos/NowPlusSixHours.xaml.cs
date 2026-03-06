using System.Threading;
using System.Windows;

namespace MathConverter.Demo.Demos;

public partial class NowPlusSixHours : Window
{
    public NowPlusSixHours()
    {
        InitializeComponent();

        var timer = new Timer(Tick, null, 1000, 1000);

        Unloaded += (_, _) => timer.Dispose();
    }
    private void Tick(object? _) => Dispatcher.Invoke(() => OneSecondTimer = !OneSecondTimer);
    public bool OneSecondTimer
    {
        get => (bool)GetValue(OneSecondTimerProperty);
        set => SetValue(OneSecondTimerProperty, value);
    }
    public static readonly DependencyProperty OneSecondTimerProperty = DependencyProperty.Register(nameof(OneSecondTimer), typeof(bool), typeof(NowPlusSixHours), new PropertyMetadata(false));

}
