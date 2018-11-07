using System.IO;
using System.Windows.Input;
using Configurator.Models;
using Configurator.Services;
using Core.Json;
using Microsoft.Win32;
using Mvvm;
using Mvvm.Commands;

namespace Configurator.ViewModels
{
    public class MainViewModel : BindableBase
    {
        private readonly Pinger _pinger = new Pinger();
        private string _configUri;
        private bool? _isConfigValid;
        public ProjectViewModel ProjectViewModel { get; } = new ProjectViewModel();
        public UpdateViewModel UpdateViewModel { get; } = new UpdateViewModel();

        public string ConfigUri
        {
            get => _configUri;
            set => SetProperty(ref _configUri, value);
        }

        public bool? IsConfigValid
        {
            get => _isConfigValid;
            set => SetProperty(ref _isConfigValid, value);
        }

        public ICommand PingConfCommand { get; private set; }
        public ICommand SaveCommand { get; private set; }
        public ICommand EditProjectCommand { get; private set; }
        public ICommand EditUpdateCommand { get; private set; }

        public MainViewModel()
        {
            SaveCommand = new DelegateCommand(OnSaveCommand);
            PingConfCommand = new DelegateCommand(async () => IsConfigValid = await _pinger.CheckPing(ConfigUri),
                () => _pinger.CanPing(ConfigUri));
            EditProjectCommand = new DelegateCommand(() => WindowService.ShowDialog(ProjectViewModel, 480));
            EditUpdateCommand = new DelegateCommand(() => WindowService.ShowDialog(UpdateViewModel, 200));
        }

        private void OnSaveCommand()
        {
            var dlg = new SaveFileDialog();
            if (dlg.ShowDialog() != true)
                return;

            var path = dlg.FileName;
            var serializer = new SettingsSerializer();
            var settings = serializer.WriteSettings(ProjectViewModel.ToJson(), UpdateViewModel.ToJson(), ConfigUri);

            File.WriteAllText(path, settings.ToStringIgnoreNull());
        }
    }
}
