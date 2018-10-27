using System;
using System.Windows;
using System.Windows.Controls;
using Microsoft.VisualBasic.Devices;

namespace LauncherFinal.Views
{
    /// <summary>
    /// Interaction logic for SettingsView.xaml
    /// </summary>
    public partial class SettingsView : UserControl
    {
        public SettingsView()
        {
            InitializeComponent();
        }

        private void FindMaxMemory(object sender, RoutedEventArgs e)
        {
            if (!(sender is Slider slider))
                return;

            var info = new ComputerInfo();
            // Даю 80% от возможной памяти (в мегобайтах)
            slider.Maximum = (int)(info.TotalPhysicalMemory / Math.Pow(2, 20) * 0.80);
        }
    }
}
