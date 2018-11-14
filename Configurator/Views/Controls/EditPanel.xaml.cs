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
    /// Interaction logic for EditPanel.xaml
    /// </summary>
    public partial class EditPanel : UserControl
    {
        public EditPanel()
        {
            InitializeComponent();
        }

        #region DPs

        public static readonly DependencyProperty AddCommandProperty = DependencyProperty.Register(
            "AddCommand", typeof(ICommand), typeof(EditPanel), new PropertyMetadata(default(ICommand)));

        public ICommand AddCommand
        {
            get { return (ICommand) GetValue(AddCommandProperty); }
            set { SetValue(AddCommandProperty, value); }
        }

        public static readonly DependencyProperty EditCommandProperty = DependencyProperty.Register(
            "EditCommand", typeof(ICommand), typeof(EditPanel), new PropertyMetadata(default(ICommand)));

        public ICommand EditCommand
        {
            get { return (ICommand) GetValue(EditCommandProperty); }
            set { SetValue(EditCommandProperty, value); }
        }

        public static readonly DependencyProperty DeleteCommandProperty = DependencyProperty.Register(
            "DeleteCommand", typeof(ICommand), typeof(EditPanel), new PropertyMetadata(default(ICommand)));

        public ICommand DeleteCommand
        {
            get { return (ICommand) GetValue(DeleteCommandProperty); }
            set { SetValue(DeleteCommandProperty, value); }
        }

        public static readonly DependencyProperty AddButtonHintProperty = DependencyProperty.Register(
            "AddButtonHint", typeof(string), typeof(EditPanel), new PropertyMetadata(default(string)));

        public string AddButtonHint
        {
            get { return (string) GetValue(AddButtonHintProperty); }
            set { SetValue(AddButtonHintProperty, value); }
        }

        public static readonly DependencyProperty CanEditProperty = DependencyProperty.Register(
            "CanEdit", typeof(bool), typeof(EditPanel), new PropertyMetadata(true));

        public bool CanEdit
        {
            get { return (bool) GetValue(CanEditProperty); }
            set { SetValue(CanEditProperty, value); }
        }

        #endregion
    }
}
