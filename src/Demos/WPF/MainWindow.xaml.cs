using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace MathConverterDemo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            // Depending on which sample is being run, we may need to initialize the ComboBox.
            if (FindName("cb") is ComboBox cb)
            {
                if (FindName("label") == null)
                {
                    cb.ItemsSource = new IndexedCollection<string> { "English", "Español", "Français", };
                }
                else
                {
                    cb.ItemsSource = Enumerable.Range(0, 6);
                }
                cb.SelectedIndex = 0;
                Loaded += delegate { cb.Focus(); cb.IsDropDownOpen = true; };
            }
        }
    }
}
