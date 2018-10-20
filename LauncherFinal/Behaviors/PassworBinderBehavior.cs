using System.Security;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;
using LauncherFinal.Helper;
using LauncherFinal.Models;

namespace LauncherFinal.Behaviors
{
    class PassworBinderBehavior : Behavior<PasswordBox>
    {
        private readonly ActionArbiter _actionArbiter = new ActionArbiter();

        protected override void OnAttached()
        {
            base.OnAttached();

            AssociatedObject.PasswordChanged += OnViewChanged;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();

            AssociatedObject.PasswordChanged -= OnViewChanged;

        }

        private void OnViewChanged(object sender, RoutedEventArgs e)
        {
            _actionArbiter.Do(() => Password = AssociatedObject.SecurePassword.Copy());
        }

        public static readonly DependencyProperty PasswordProperty = DependencyProperty.Register(
            "Password", typeof(SecureString), typeof(PassworBinderBehavior),
            new PropertyMetadata(default(SecureString), OnBindingChanged));

        public SecureString Password
        {
            get { return (SecureString) GetValue(PasswordProperty); }
            set { SetValue(PasswordProperty, value); }
        }

        private static void OnBindingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is PassworBinderBehavior behavior))
                return;

            behavior._actionArbiter.Do(() =>
            {
                behavior.AssociatedObject.Password = behavior.Password.ConvertToString();
            });
        }
    }
}
