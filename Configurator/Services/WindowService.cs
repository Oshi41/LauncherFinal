using System;
using System.Windows.Input;
using Configurator.Views.Base;
using Mvvm;

namespace Configurator.Services
{
    public static class WindowService
    {
        private static bool? ShowDialogInner(BindableBase vm,
            double width,
            double heigth,
            Predicate<object> canSave)
        {
            var dlg = new DialogWindow
            {
                DataContext = vm,

                MinHeight = heigth,
                MinWidth = width,

                Width = width,
                Height = heigth,
            };

            var cmd = new CommandBinding(
                ApplicationCommands.Save,
                (sender, args) => { },
                (sender, args) => args.CanExecute = canSave == null || canSave(args.Parameter));

            dlg.CommandBindings.Add(cmd);

            return dlg.ShowDialog();

        }

        public static bool? ShowVerticalDialog(BindableBase vm, 
            int width, 
            Predicate<object> canSave = null)
        {
            return ShowDialogInner(vm, width, width * 1.25, canSave);
        }

        public static bool? ShowHorizontalialog(BindableBase vm,
            int height,
            Predicate<object> canSave = null)
        {
            return ShowDialogInner(vm, height * 1.25, height, canSave);
        }
    }
}
