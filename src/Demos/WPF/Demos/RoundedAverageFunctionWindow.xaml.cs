using HexInnovation;
using MathConverter.Demo.CustomFunctions;
using System.Windows;
using System.Windows.Controls;

namespace MathConverter.Demo.Demos;

public partial class RoundedAverageFunctionWindow : Window
{
    public RoundedAverageFunctionWindow() => InitializeComponent();

    private void RadioButton_Changed(object sender, RoutedEventArgs e)
    {
        if (FindResource("Math") is not HexInnovation.MathConverter math)
            return;

        if (UseStockFunction.IsChecked == true)
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

        // Tell the TextBlock to refresh its binding again.
        TextBlock?.GetBindingExpression(TextBlock.TextProperty).UpdateTarget();
    }
}
