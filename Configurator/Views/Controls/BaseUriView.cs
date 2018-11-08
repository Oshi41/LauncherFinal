using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Mvvm.Commands;

namespace Configurator.Views.Controls
{
    [TemplatePart(Name = ButtonName, Type = typeof(Button))]
    [TemplatePart(Name = TextName, Type = typeof(TextBox))]
    class BaseUriView : UserControl
    {
        protected const string ButtonName = "PingButton";
        protected const string TextName = "UriBox";

        protected Func<string, Task<bool>> Action;
        protected Func<string, bool> CanPing;

        public static readonly DependencyProperty IsCheckedPathProperty = DependencyProperty.Register(
            "IsCheckedPath", typeof(bool?), typeof(BaseUriView), new PropertyMetadata(default(bool?)));

        public bool? IsCheckedPath
        {
            get { return (bool) GetValue(IsCheckedPathProperty); }
            set { SetValue(IsCheckedPathProperty, value); }
        }

        public static readonly DependencyProperty ButtonHintProperty = DependencyProperty.Register(
            "ButtonHint", typeof(string), typeof(BaseUriView), new PropertyMetadata(default(string)));

        public string ButtonHint
        {
            get { return (string) GetValue(ButtonHintProperty); }
            set { SetValue(ButtonHintProperty, value); }
        }

        public static readonly DependencyProperty UriHintProperty = DependencyProperty.Register(
            "UriHint", typeof(string), typeof(BaseUriView), new PropertyMetadata(default(string)));

        public string UriHint
        {
            get { return (string) GetValue(UriHintProperty); }
            set { SetValue(UriHintProperty, value); }
        }

        public static readonly DependencyProperty PathProperty = DependencyProperty.Register(
            "Path", typeof(string), typeof(BaseUriView), 
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public string Path
        {
            get { return (string) GetValue(PathProperty); }
            set { SetValue(PathProperty, value); }
        }

        protected BaseUriView(Func<string, Task<bool>> action, Func<string, bool> canPing)
        {
            Action = action ?? (s => new Task<bool>(() => true));
            CanPing = canPing;
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            var command = DelegateCommand.FromAsyncHandler(async () =>
                {
                    IsCheckedPath = await Action(Path);

                    if (Button.Command is DelegateCommand c)
                    {
                        c.RaiseCanExecuteChanged();
                    }
                },
                () => CanPing?.Invoke(Path) ?? true);


            Button.Command = command;
        }

        protected Button Button
        {
            get
            {
                var button = (Button)Template.FindName(ButtonName, this);
                if (button == null)
                    throw new NullReferenceException(nameof(button));
                return button;
            }
        }
    }
}
