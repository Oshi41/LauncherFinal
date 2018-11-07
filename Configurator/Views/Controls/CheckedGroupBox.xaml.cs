using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Configurator.Views.Controls
{
    /// <summary>
    /// Interaction logic for CheckedGroupBox.xaml
    /// </summary>
    public partial class CheckedGroupBox : UserControl
    {
        public CheckedGroupBox()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty IsCheckedProperty = DependencyProperty.Register(
            "IsChecked", typeof(bool), typeof(CheckedGroupBox), new PropertyMetadata(default(bool)));

        public bool IsChecked
        {
            get { return (bool) GetValue(IsCheckedProperty); }
            set { SetValue(IsCheckedProperty, value); }
        }

        public static readonly DependencyProperty HeaderProperty = DependencyProperty.Register(
            "Header", typeof(string), typeof(CheckedGroupBox), new PropertyMetadata(default(string)));

        public string Header
        {
            get { return (string) GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }
    }
}
