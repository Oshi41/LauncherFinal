using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Windows.Input;
using LauncherCore.Settings;
using LauncherFinal.ViewModels.PopupViewModels;
using Microsoft.Win32;
using Mvvm;
using Mvvm.Commands;

namespace LauncherFinal.ViewModels
{
    class LauncherSettingsViewModel : BindableBase
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

        public Version CurrentVersion { get;  }

        #endregion

        #region Commands

        public ICommand SaveCommand { get; }
        public ICommand UpdateLauncherCommand { get; private set; }
        public ICommand ClearFolderCommand { get; private set; }
        public ICommand OpenProjectSiteCommand { get; private set; }
        public ICommand OpenBaseFolderCommand { get; private set; }
        public ICommand FindJavaCommand { get; private set; }
        public ICommand DefaultSettingsCommand { get; private set; }

        #endregion

        public LauncherSettingsViewModel(ISettings settings)
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

            FindJavaCommand = new DelegateCommand(OnFindJavaPath);

            DefaultSettingsCommand = new DelegateCommand(BackToDefaultsCommand);

            CurrentVersion = Assembly.GetExecutingAssembly().GetName().Version;

            Refresh();
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
            var hostName = "LauncherSettingsViewModel";
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
                await MessageService.ShowMessage("Отсутствует файл обновлений - " + updatePath);
                return;
            }

            Process.Start(updatePath, $"{file} {currentExeName} {currentThreadName}");
            Process.GetCurrentProcess().Kill();
        }

        private void OnSave()
        {
            _settings.JavaPath = JavaPath;
            _settings.Megabytes = Megobytes;
            _settings.OptimizeJava = OptimizeJava;
            _settings.ClientFolder = ClientFolder;
        }

        private async void OnClearCommand()
        {
            var result = await MessageService.ShowDialog(
                "Файлы будут удалены навсегда (что значит очень долго).\nПродолжить?",
                false);

            if (result != true)
                return;

            try
            {
                // clear all servers folders!!!
                if (Directory.Exists(_settings.ClientFolder))
                    Directory.Delete(_settings.ClientFolder, true);

            }
            catch (Exception e)
            {
                Trace.Write(e);
                await MessageService.ShowMessage(e.Message);
            }
        }

        private void OnFindJavaPath()
        {
            var dlg = new OpenFileDialog
            {
                CheckFileExists = true,
                InitialDirectory = JavaPath,
                Filter = "Исполняемые файлы|*.exe",
                Multiselect = false
            };

            if (dlg.ShowDialog() == true)
            {
                JavaPath = dlg.FileName;
            }
        }

        private async void BackToDefaultsCommand()
        {
            var result = await MessageService.ShowDialog(
                "Вернуться к настройка по умолчанию?", true);

            if (result != true)
                return;

            var worker = IoCContainer.Instance.Resolve<ISettingsWorker>();
            worker.BackToDefaults();
        }

        #endregion

        #region Methods

        private void Refresh()
        {
            JavaPath = _settings.JavaPath;
            Megobytes = _settings.Megabytes;
            OptimizeJava = _settings.OptimizeJava;
            ClientFolder = _settings.ClientFolder;

            var latest = _settings?.UpdateConfig?.Version ?? new Version();

            _needToUpdate = latest > CurrentVersion;
            _emptyFolder = !Directory.GetFiles(_settings.ClientFolder).Any();
        }

        #endregion
    }
}

