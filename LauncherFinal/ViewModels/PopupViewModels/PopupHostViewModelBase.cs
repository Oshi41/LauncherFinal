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

        protected PopupHostViewModelBase(string dialogHostName, bool canClickAway)
        {
            DialogHostName = dialogHostName;
            CanClickAway = canClickAway;
        }

        public ICommand CloseCommand => DialogHost.CloseDialogCommand;

        public bool CanClickAway { get; }

        public void ClosePopup()
        {
            CloseCommand.Execute(this);
        }

        public void ShowPopup()
        {
            DialogHost.Show(this, DialogHostName);
        }
    }
}
