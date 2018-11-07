using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interactivity;

namespace LauncherFinal.Behaviors
{
    class DragAndDropBehavior : Behavior<Window>
    {
        private readonly List<Type> _restricted = new List<Type>
        {
            typeof(IInputElement),
        };

        protected override void OnAttached()
        {
            base.OnAttached();

            AssociatedObject.MouseLeftButtonDown += OnDragAndDrop;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();

            AssociatedObject.MouseLeftButtonDown -= OnDragAndDrop;
        }

        private void OnDragAndDrop(object sender, MouseButtonEventArgs e)
        {
            if (!_restricted.Contains(e.OriginalSource.GetType()))
            {
                AssociatedObject.DragMove();
            }
        }
    }
}
