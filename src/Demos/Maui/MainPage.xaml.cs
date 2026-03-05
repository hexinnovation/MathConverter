namespace MathConverter.Demo;

public partial class MainPage : ContentPage
{
    public MainPage() => InitializeComponent();

    private async void Item_Tapped(object sender, TappedEventArgs e)
    {
        if (e.Buttons == ButtonsMask.Primary && sender is Label { BindingContext: Type type } && type.IsAssignableTo(typeof(Page)))
        {
            var page = (Page)Activator.CreateInstance(type)!;
            await Navigation.PushAsync(page);
        }
    }
}
