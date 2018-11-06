using System;
using System.Windows;
using System.Windows.Input;
using Configurator.Views.Base;
using Mvvm;

namespace Configurator.Services
{
    public static class WindowService
    {
        public static bool? ShowDialog(BindableBase vm, 
            Size minsize, 
            Predicate<object> canSave = null)
        {
            var dlg = new DialogWindow
            {
                DataContext = vm,

                MinHeight = minsize.Height,
                MinWidth = minsize.Width,

                Width = minsize.Width,
                Height = minsize.Height,
            };

            var cmd = new CommandBinding(
                ApplicationCommands.Save,
                (sender, args) => {},
                (sender, args) => args.CanExecute = canSave == null || canSave(args.Parameter));

            dlg.CommandBindings.Add(cmd);

            return dlg.ShowDialog();
        }

    }
}
