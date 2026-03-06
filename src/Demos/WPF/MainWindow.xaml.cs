using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MathConverter.Demo;

public partial class MainWindow : Window
{
    public MainWindow() => InitializeComponent();

    private void Item_DoubleClick(object sender, MouseButtonEventArgs e)
    {
        if (e.ChangedButton == MouseButton.Left && sender is ListBoxItem { DataContext: Type type } && type.IsAssignableTo(typeof(Window)))
        {
            if (Activator.CreateInstance(type) is Window window)
            {
                window.ShowDialog();
            }
        }
    }
}
