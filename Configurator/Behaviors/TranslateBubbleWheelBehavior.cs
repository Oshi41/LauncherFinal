using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interactivity;
using Core.Helper;

namespace Configurator.Behaviors
{
    public class TranslateBubbleWheelBehavior : Behavior<UIElement>
    {
        protected override void OnAttached()
        {
            base.OnAttached();

            AssociatedObject.PreviewMouseWheel += OnMouseWheel;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();

            AssociatedObject.PreviewMouseWheel += OnMouseWheel;
        }

        private void OnMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (!(e.OriginalSource is DependencyObject original))
                return;

                                                            // Да, дельта больше нуля - верх
            var scroll = FindFirstMoovableScroll(original, e.Delta > 0);
            if (scroll == null)
                return;

            e.Handled = true;
            var e2 = new MouseWheelEventArgs(e.MouseDevice, e.Timestamp, e.Delta)
            {
                RoutedEvent = UIElement.MouseWheelEvent
            };

            scroll.RaiseEvent(e2);
        }

        /// <summary>
        /// Возаращает первый доступный скролл, который можно прокрутить
        /// </summary>
        /// <param name="element"></param>
        /// <param name="onTop"></param>
        /// <returns></returns>
        private ScrollViewer FindFirstMoovableScroll(DependencyObject element, bool onTop)
        {
            if (element == null)
                return null;

            if (element is ScrollViewer viewer)
            {
                return CannotMove(viewer, onTop)
                    ? FindFirstMoovableScroll(WpfHelper.GetParent<ScrollViewer>(viewer), onTop)
                    : viewer;
            }

            return FindFirstMoovableScroll(WpfHelper.GetParent<ScrollViewer>(element), onTop);
        }

        /// <summary>
        /// Проверяет, если скроллвьювер прокручен до конца (верх или низ)
        /// </summary>
        /// <param name="viewer"></param>
        /// <param name="onTop"></param>
        /// <returns></returns>
        private bool CannotMove(ScrollViewer viewer, bool onTop)
        {
            return
                // упёрлись вверх
                (onTop && viewer.VerticalOffset == 0)
                ||
                // упёрлись вниз
                (!onTop && viewer.ScrollableHeight == viewer.VerticalOffset);
        }
    }
}
