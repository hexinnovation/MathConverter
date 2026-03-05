namespace MathConverter.Demo.Demos;

public partial class BooleanToVisibilityPage : ContentPage
{
    public BooleanToVisibilityPage() => InitializeComponent();

    private void Label_Tapped(object sender, TappedEventArgs e) => CheckBox.IsChecked = !CheckBox.IsChecked;
}
