using HexInnovation;
using MathConverter.Demo.CustomFunctions;

namespace MathConverter.Demo.Demos;

public partial class RoundedAverageFunctionPage : ContentPage
{
    public RoundedAverageFunctionPage() => InitializeComponent();

    private void RadioButton_Changed(object? _, CheckedChangedEventArgs e)
    {
        if (!Resources.TryGetValue("Math", out var m) || m is not HexInnovation.MathConverter math)
            return;

        if (UseStockFunction.IsChecked)
        {
            // Go back to the stock function.
            math.CustomFunctions.Clear();
            math.CustomFunctions.RegisterDefaultFunctions();
        }
        else
        {
            // Remove the default Average function and define our own.
            math.CustomFunctions.Remove("Average");
            math.CustomFunctions.Add(CustomFunctionDefinition.Create<RoundedAverageFunction>("Average"));
        }

        // Tell the Label to refresh its binding again.
        RefreshBinding = !RefreshBinding;
    }

    public bool RefreshBinding
    {
        get => (bool)GetValue(RefreshBindingProperty);
        set => SetValue(RefreshBindingProperty, value);
    }
    public static readonly BindableProperty RefreshBindingProperty = BindableProperty.Create(nameof(RefreshBinding), typeof(bool), typeof(RoundedAverageFunctionPage), false);
}
