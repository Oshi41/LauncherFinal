using System.Windows;
using System.Windows.Controls;

namespace LauncherFinal.Views
{
    /// <summary>
    /// Interaction logic for FlipPanel.xaml
    /// </summary>
    public partial class FlipPanel : UserControl
    {
        public FlipPanel()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty IsFlippedProperty = DependencyProperty.Register(
            "IsFlipped", typeof(bool), typeof(FlipPanel), new FrameworkPropertyMetadata(false,
                FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public bool IsFlipped
        {
            get { return (bool) GetValue(IsFlippedProperty); }
            set { SetValue(IsFlippedProperty, value); }
        }

        public static readonly DependencyProperty FrontProperty = DependencyProperty.Register(
            "Front", typeof(object), typeof(FlipPanel), new PropertyMetadata(default(object)));

        public object Front
        {
            get { return (object) GetValue(FrontProperty); }
            set { SetValue(FrontProperty, value); }
        }

        public static readonly DependencyProperty BackProperty = DependencyProperty.Register(
            "Back", typeof(object), typeof(FlipPanel), new PropertyMetadata(default(object)));

        public object Back
        {
            get { return (object) GetValue(BackProperty); }
            set { SetValue(BackProperty, value); }
        }
    }
}
