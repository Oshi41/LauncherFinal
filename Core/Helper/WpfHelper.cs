using System.Windows;
using System.Windows.Media;

namespace Core.Helper
{
    public static class WpfHelper
    {

        public static T GetParent<T>(DependencyObject child)
            where T : DependencyObject
        {
            if (child == null)
                return null;

            var parent = VisualTreeHelper.GetParent(child) ?? LogicalTreeHelper.GetParent(child);

            if (parent is T result)
            {
                return result;
            }

            return GetParent<T>(parent);
        }
    }
}
