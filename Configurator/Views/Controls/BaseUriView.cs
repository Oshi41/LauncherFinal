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

        protected Func<string, Task<bool>> _action;
        protected Func<string, bool> _canPing;

        public static readonly DependencyProperty IsCheckedPathProperty = DependencyProperty.Register(
            "IsCheckedPath", typeof(bool?), typeof(BaseUriView), new PropertyMetadata(default(bool?)));

        public bool? IsCheckedPath
        {
            get { return (bool) GetValue(IsCheckedPathProperty); }
            set { SetValue(IsCheckedPathProperty, value); }
        }

        public static readonly DependencyProperty ButtonToolTipProperty = DependencyProperty.Register(
            "ButtonToolTip", typeof(string), typeof(BaseUriView), new PropertyMetadata(default(string)));

        public string ButtonToolTip
        {
            get { return (string) GetValue(ButtonToolTipProperty); }
            set { SetValue(ButtonToolTipProperty, value); }
        }

        protected BaseUriView(Func<string, Task<bool>> action, Func<string, bool> canPing)
        {
            _action = action ?? (s => new Task<bool>(() => true));
            _canPing = canPing;
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            var command = DelegateCommand.FromAsyncHandler(async () =>
                {
                    IsCheckedPath = await _action(Uri);
                },
                () => _canPing?.Invoke(Uri) ?? true);

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

        protected string Uri
        {
            get
            {
                var box = (TextBox)Template.FindName(TextName, this);
                return box?.Text;
            }
        }
    }
}
