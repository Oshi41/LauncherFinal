using System.Linq;
using System.Windows;
using LauncherFinal.ViewModels.PopupViewModels;

namespace LauncherFinal
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void AfterLoaded(object sender, RoutedEventArgs e)
        {
            var text = string.Join("\n", Enumerable.Repeat("Since one of StringEnumConverter's constructors takes a boolean for", 100));

            MessageService.ShowMessage("1", text);
        }
    }
}
