using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Windows.Input;
using LauncherFinal.Models.Settings.Interfases;
using LauncherFinal.ViewModels.PopupViewModels;
using Mvvm;
using Mvvm.Commands;

namespace LauncherFinal.ViewModels
{
    class LauncherSettings : BindableBase
    {
        #region Fields

        private readonly ISettings _settings;

        private string _javaPath;
        private int _megobytes;
        private bool _optimizeJava;
        private string _clientFolder;

        private bool _needToUpdate;
        private bool _emptyFolder;

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
        public ICommand OpenBaseFolderCommand { get; private set; }

        #endregion

        public LauncherSettings(ISettings settings)
        {
            _settings = settings;
            _settings.OnSettingsChanged += OnSettingChanged;

            OpenProjectSiteCommand = new DelegateCommand(
                () => Process.Start(_settings?.ProjectConfig?.ProjectSite),
                () => !string.IsNullOrWhiteSpace(_settings?.ProjectConfig?.ProjectSite));

            OpenBaseFolderCommand = new DelegateCommand(
                () => Process.Start("explorer", _settings?.ClientFolder),
                () => !string.IsNullOrWhiteSpace(_settings?.ClientFolder));

            ClearFolderCommand = new DelegateCommand(OnClearCommand, () => !_emptyFolder);

            SaveCommand = new DelegateCommand(OnSave);

            UpdateLauncherCommand = new DelegateCommand(OnUpdate, () => _needToUpdate);
        }

        #region Event handlers

        private void OnSettingChanged(object sender, EventArgs e)
        {
            Refresh();
        }

        #endregion

        #region Command handlers

        private async void OnUpdate()
        {
            // todo map DialogHostNames
            var hostName = "LauncherSettings";
            DownloadViewModel vm = null;

            try
            {
                vm = new DownloadViewModel(hostName, _settings?.UpdateConfig?.ExeUrl);
            }
            catch (Exception e)
            {
                Trace.Write(e);
                return;
            }

            // передаваемые аргументы
            var file = await vm.Start();
            var currentExeName = AppDomain.CurrentDomain.FriendlyName;
            var currentThreadName = Thread.CurrentThread.Name;

            var updatePath = Path.Combine(_settings.ClientFolder, "..", "update", "update.exe");

            if (!File.Exists(updatePath))
            {
                Trace.WriteLine("Can't find updator. Check or reinstall");
                //todo show error for user
                return;
            }

            Process.Start(updatePath, $"{file} {currentExeName} {currentThreadName}");
            Process.GetCurrentProcess().Kill();
        }

        private void OnSave()
        {
            _settings.JavaPath = JavaPath;
            _settings.Megobytes = Megobytes;
            _settings.OptimizeJava = OptimizeJava;
            _settings.ClientFolder = ClientFolder;
        }

        private void OnClearCommand()
        {
            // todo ask user

            try
            {
                // clear all servers folders!!!
                if (Directory.Exists(_settings.ClientFolder))
                    Directory.Delete(_settings.ClientFolder, true);

            }
            catch (Exception e)
            {
                Trace.Write(e);
                // todo show to user
            }
        }

        #endregion

        #region Methods

        private void Refresh()
        {
            JavaPath = _settings.JavaPath;
            Megobytes = _settings.Megobytes;
            OptimizeJava = _settings.OptimizeJava;
            ClientFolder = _settings.ClientFolder;

            var current = Assembly.GetExecutingAssembly().GetName().Version;
            var latest = _settings.UpdateConfig.Version;

            _needToUpdate = latest > current;
            _emptyFolder = !Directory.GetFiles(_settings.ClientFolder).Any();
        }

        #endregion
    }
}

