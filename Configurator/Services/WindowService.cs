using System;
using System.Windows.Input;
using Configurator.Views.Base;
using Mvvm;

namespace Configurator.Services
{
    public static class WindowService
    {
        public static bool? ShowDialog(BindableBase vm, 
            int width, 
            Predicate<object> canSave = null)
        {
            var dlg = new DialogWindow
            {
                DataContext = vm,

                MinHeight = width* 1.25,
                MinWidth = width,

                Width = width,
                Height = width* 1.25,
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
