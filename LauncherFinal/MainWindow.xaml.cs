using System.Windows;

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

        private void OnClose(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void OnMinimize(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void Flip(object sender, RoutedEventArgs e)
        {
            Flipper.IsFlipped = !Flipper.IsFlipped;
        }
    }
}
