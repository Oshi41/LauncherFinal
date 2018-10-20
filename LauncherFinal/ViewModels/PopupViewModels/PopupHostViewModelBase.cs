using System.Windows.Input;
using MaterialDesignThemes.Wpf;
using Mvvm;

namespace LauncherFinal.ViewModels.PopupViewModels
{
    public interface IPopupViewModel
    {
        void ShowPopup();
        void ClosePopup();
        ICommand CloseCommand { get; }
    }

    abstract class PopupHostViewModelBase : BindableBase, IPopupViewModel
    {
        protected readonly string DialogHostName;

        protected PopupHostViewModelBase(string dialogHostName)
        {
            DialogHostName = dialogHostName;
        }

        public ICommand CloseCommand => DialogHost.CloseDialogCommand;

        public void ClosePopup()
        {
            DialogHost.Show(this, DialogHostName);
        }

        public void ShowPopup()
        {
            CloseCommand.Execute(this);
        }
    }
}
