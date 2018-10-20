using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using LauncherFinal.Models.Settings.Interfases;
using Mvvm;
using Mvvm.Commands;

namespace LauncherFinal.ViewModels
{
    class LauncherSettings : BindableBase
    {
        private readonly ISettings _settings;

        #region Fields
        
        private string _javaPath;
        private int _megobytes;
        private bool _optimizeJava;
        private string _clientFolder;

        #endregion

        #region Properties

        public string JavaPath
        {
            get => _javaPath;
            set => SetProperty(ref _javaPath, value);
        }

        public int Megobytes
        {
            get => _megobytes;
            set => SetProperty(ref _megobytes, value);
        }

        public bool OptimizeJava
        {
            get => _optimizeJava;
            set => SetProperty(ref _optimizeJava, value);
        }

        public string ClientFolder
        {
            get => _clientFolder;
            set => SetProperty(ref _clientFolder, value);
        }

        #endregion

        #region Commands

        public ICommand SaveCommand { get; }
        public ICommand UpdateLauncherCommand { get; private set; }
        public ICommand ClearFolderCommand { get; private set; }
        public ICommand OpenProjectSiteCommand { get; private set; }

        #endregion

        public LauncherSettings(ISettings settings)
        {
            _settings = settings;

            OpenProjectSiteCommand = new DelegateCommand(
                () => Process.Start(_settings?.ProjectConfig?.ProjectSite),
                () => !string.IsNullOrWhiteSpace(_settings?.ProjectConfig?.ProjectSite));

            SaveCommand = new DelegateCommand(OnSave);

            UpdateLauncherCommand = new DelegateCommand(OnUpdate);
        }

        private void OnUpdate()
        {
            var uri = _settings?.UpdateConfig?.ExeUrl;
            // todo Make DownloadViewModel
        }

        #region Command Handlers

        private void OnSave()
        {
            _settings.JavaPath = JavaPath;
            _settings.Megobytes = Megobytes;
            _settings.OptimizeJava = OptimizeJava;
            _settings.ClientFolder = ClientFolder;
        }

        #endregion
    }
}
