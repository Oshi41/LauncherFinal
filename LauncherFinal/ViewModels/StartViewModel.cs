using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using LauncherFinal.Helper;
using LauncherFinal.Models;
using LauncherFinal.Models.AuthModules;
using LauncherFinal.Models.Settings;
using LauncherFinal.Models.Settings.Interfases;
using LauncherFinal.ViewModels.PopupViewModels;
using Mvvm;
using Mvvm.Commands;

namespace LauncherFinal.ViewModels
{
    public class StartViewModel : BindableBase
    {
        #region Fields

        private readonly ISettings _settings;
        private readonly AuthModuleFactory _factory = new AuthModuleFactory();
        private readonly CryptoWorker _crypter = new CryptoWorker();

        private ObservableCollection<ServerViewModel> _servers = new ObservableCollection<ServerViewModel>();
        private ServerViewModel _choosenServer;
        private ModuleTypes _selectedAuthModule;
        private string _login;
        private SecureString _password;
        private bool _rememberPassword;
        private List<ModuleTypes> _modules = new List<ModuleTypes>();

        #endregion

        #region Properties

        public ObservableCollection<ServerViewModel> Servers
        {
            get => _servers;
            set => SetProperty(ref _servers, value);
        }

        public ServerViewModel ChoosenServer
        {
            get => _choosenServer;
            set => SetProperty(ref _choosenServer, value);
        }

        public List<ModuleTypes> Modules
        {
            get => _modules;
            set => SetProperty(ref _modules, value);
        }

        public ModuleTypes SelectedAuthModule
        {
            get => _selectedAuthModule;
            set => SetProperty(ref _selectedAuthModule, value);
        }

        public string Login
        {
            get => _login;
            set => SetProperty(ref _login, value);
        }

        public SecureString Password
        {
            get => _password;
            set => SetProperty(ref _password, value);
        }

        public bool RememberPassword
        {
            get => _rememberPassword;
            set => SetProperty(ref _rememberPassword, value);
        }

        public ICommand LaunchCommand { get; }
        public ICommand RefreshCommand { get; }

        #endregion

        public StartViewModel()
        {
            _settings = IoCContainer.Instance.Resolve<ISettings>();

            _settings.OnSettingsChanged += AfterSettingsChanged;

            if (Application.Current.MainWindow != null)
                Application.Current.MainWindow.Closing += OnSaveChanges;

            Refresh(_settings.ProjectConfig);

            RefreshCommand = new DelegateCommand(() => Refresh(_settings.ProjectConfig));
            LaunchCommand = new DelegateCommand(OnLaunch, OnCanLaunch);
        }

        #region Command Handlers

        private bool OnCanLaunch()
        {
            return ChoosenServer?.StateInfo?.ServerUp == true;
        }

        private async void OnLaunch()
        {
            SaveSettings();

            var folder = Path.Combine(_settings.ClientFolder, ChoosenServer.Name);

            if (!await CheckAndDownload(ChoosenServer.DownloadLink, folder))
            {
                await MessageService.ShowMessage("Ошибка в скачивании клиентских файлов");
                return;
            }

            if (!CheckHash(folder, ChoosenServer.DirHashCheck))
            {
                var canReinstall = await MessageService.ShowDialog(
                    "Хэш-сумма папки не совпдает, требуется переустановка.\nПродолжить?",
                    false);

                if (canReinstall == true)
                {
                    Directory.Delete(folder, true);
                    // заного запускаю метод
                    OnLaunch();
                }

                return;
            }

            var module = _factory.GetByType(SelectedAuthModule);
            var result = await module.GenerateToken(Login, Password);
            if (!result.Key)
            {
                await MessageService.ShowMessage("Ошибка в регистрации");
                return;
            }

            try
            {
                var launcher = new ForgeLaunchWorker(folder,
                    result.Value,
                    _settings.JavaPath,
                    Login,
                    _settings.OptimizeJava);

                launcher.RegularLaunch(OnExit);

                Application.Current.MainWindow?.Hide();
            }
            catch (Exception e)
            {
                Trace.Write(e);
                await MessageService.ShowMessage(e.Message);
            }
        }

        private void OnExit(object sender, EventArgs e)
        {
            Application.Current.MainWindow?.Show();
        }

        #endregion

        private void AfterSettingsChanged(object sender, EventArgs e)
        {
            if (sender is IProjectConfig config)
                Refresh(config);
        }

        private void OnSaveChanges(object sender, EventArgs e)
        {
            SaveSettings();
        }

        #region Methods

        private void Refresh(IProjectConfig config)
        {
            Servers.Clear();
            SetModules(config);
            PassFromSettings();
            Login = _settings.Login;
            RememberPassword = _settings.SavePass;

            var choosen = ChoosenServer;

            if (config?.Servers?.Any() == true)
            {
                var servers = config
                    .Servers
                    .Select(x => new ServerViewModel(x));

                Servers = new ObservableCollection<ServerViewModel>(servers);

                if (choosen != null)
                    ChoosenServer = Servers.FirstOrDefault(x => Equals(choosen, x));

                // запускаем в самом конце
                Servers.ForEach(async x => await x.Ping());
            }
        }

        private void SaveSettings()
        {
            SavePassToSettings();
            _settings.Login = Login;
            _settings.SavePass = RememberPassword;
        }

        private async Task<bool> CheckAndDownload(string url, string folder)
        {
            if (Directory.Exists(folder) && Directory.GetFiles(folder).Any())
                return true;

            // todo Вставить нормальное имя хоста
            var hostName = "StartViewModel";

            var vm = new DownloadViewModel(hostName, url);
            var filePath = await vm.Start();

            if (vm.IsError)
                return false;

            var compress = new CompressionViewModel(hostName, filePath, folder, true, true, true);
            return await compress.Extract();
        }

        private bool CheckHash(string folder, IDictionary<string, string> md5)
        {
            if (!Directory.Exists(folder) || md5.IsNullOrEmpty())
                return true;

            var worker = new HashChecker();

            foreach (var hash in md5)
            {
                var path = Path.GetFullPath(Path.Combine(folder, hash.Key));
                var folderHash = worker.CreateMd5ForFolder(path);
                if (!string.Equals(hash.Value, folderHash))
                    return false;
            }

            return true;
        }

        private void PassFromSettings()
        {
            if (_settings.Password == null)
                return;

            var pass = _crypter.Decrypt<AesManaged>(_settings.Password.Encrypted,
                _settings.Password.Password, _settings.Password.Salt);

            var secure = new SecureString();
            pass.ForEach(x => secure.AppendChar(x));
            Password = secure;
        }

        private void SavePassToSettings()
        {
            var password = new PasswordSettings();

            if (RememberPassword)
            {
                password.Password = _crypter.GetRandomSalt();
                password.Salt = _crypter.GetRandomSalt();
                password.Encrypted = _crypter.Encrypt<AesManaged>(Password.ConvertToString(),
                    password.Password,
                    password.Salt);
            }

            _settings.Password = password;
        }

        private void SetModules(IProjectConfig config)
        {
            Modules.Clear();
            var modules = new List<ModuleTypes>();
            modules.AddRange(Enum.GetValues(typeof(ModuleTypes)).OfType<ModuleTypes>());

            if (config?.AuthModuleSettings == null)
            {
                modules.Remove(ModuleTypes.Custom);
            }
            else
            {
                if (config.AuthModuleSettings.StrictUsage)
                {
                    modules.RemoveAll(x => x != ModuleTypes.Custom);
                }
            }

            Modules = modules;
        }

        #endregion
    }
}
