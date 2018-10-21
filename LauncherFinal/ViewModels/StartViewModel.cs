using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using LauncherFinal.Helper;
using LauncherFinal.Models;
using LauncherFinal.Models.AuthModules;
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

        private ObservableCollection<ServerViewModel> _servers;
        private ServerViewModel _choosenServer;
        private ModuleTypes _authModule;
        private string _login;
        private SecureString _password;
        private bool _rememberPassword;

        #endregion

        #region Properties

        public ObservableCollection<ServerViewModel> Servers
        {
            get { return _servers; }
            set { SetProperty(ref _servers, value); }
        }

        public ServerViewModel ChoosenServer
        {
            get => _choosenServer;
            set => SetProperty(ref _choosenServer, value);
        }

        public ModuleTypes AuthModule
        {
            get { return _authModule; }
            set { SetProperty(ref _authModule, value); }
        }

        public string Login
        {
            get { return _login; }
            set { SetProperty(ref _login, value); }
        }

        public SecureString Password
        {
            get { return _password; }
            set { SetProperty(ref _password, value); }
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

            // todo придумать как назвать хост
            var messageHostName = "MessageHost";

            if (!await CheckAndDownload(ChoosenServer.DownloadLink, folder))
            {
                await MessageService.ShowMessage(messageHostName, "Ошибка в скачивании клиентских файлов");
                return;
            }

            if (!CheckHash(folder, ChoosenServer.DirHashCheck))
            {
                var canReinstall = await MessageService.ShowDialog(messageHostName,
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

            var module = _factory.GetByType(AuthModule);
            var result = await module.GenerateToken(Login, Password);
            if (!result.Key)
            {
                await MessageService.ShowMessage(messageHostName, "Ошибка в регистрации");
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
                await MessageService.ShowMessage(messageHostName, e.Message);
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

        #region Methods

        private void Refresh(IProjectConfig config)
        {
            PassFromSettings();
            Login = _settings.Login;
            RememberPassword = _settings.SavePass;

            var choosen = ChoosenServer;

            var servers = config
                .Servers
                .Select(x => new ServerViewModel(x));

            Servers = new ObservableCollection<ServerViewModel>(servers);

            ChoosenServer = Servers.FirstOrDefault(x => Equals(choosen, x));

            Servers.ForEach(async x => await x.Ping());
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
            var unique = _crypter.GetUniqueSalt();
            var unique2 = _crypter.GetNextUniqueSalt(unique);
            var pass = _crypter.Decrypt<AesManaged>(_settings.Password, unique, unique2);

            var secure = new SecureString();
            pass.ForEach(x => secure.AppendChar(x));
            Password = secure;
        }

        private void SavePassToSettings()
        {
            var unique = _crypter.GetUniqueSalt();
            var unique2 = _crypter.GetNextUniqueSalt(unique);

            _settings.Password = _crypter.Encrypt<AesManaged>(Password.ConvertToString(), unique, unique2);
        }

        #endregion
    }
}
