﻿using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Input;
using MaterialDesignThemes.Wpf;
using Mvvm.Commands;

namespace LauncherFinal.ViewModels.PopupViewModels
{
    public static class MessageService
    {
        public static async Task ShowMessage(string hostName, string message)
        {
            var vm = new MessageViewModel(message);
            await DialogHost.Show(hostName, vm);
        }

        public static async Task<bool?> ShowDialog(string hostName, string question, bool isCancable)
        {
            var vm = new DialogViewModel(question, isCancable);
            await DialogHost.Show(hostName, vm);
            return vm.DialodResult;
        }
    }

    class MessageViewModel
    {
        public MessageViewModel(string message)
        {
            Message = message;
        }

        public string Message { get; }
        public ICommand CloseCommand => DialogHost.CloseDialogCommand;
    }

    class DialogViewModel
    {
        public DialogViewModel(string question, bool isCancable)
        {
            Question = question;
            IsCancable = isCancable;
            YesCommand = new DelegateCommand(() => SetResult(true));
            NoCommand = new DelegateCommand(() => SetResult(false));
        }

        public string Question { get; }
        public bool IsCancable { get; }
        public bool? DialodResult { get; private set; }
        public ICommand YesCommand { get; private set; }
        public ICommand NoCommand { get; private set; }
        public ICommand CancelCommand => DialogHost.CloseDialogCommand;

        private void SetResult(bool? result)
        {
            DialodResult = result;
            CancelCommand.Execute(this);
        }
    }
}