using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Security;
using System.Security.Cryptography;
using System.Windows;
using System.Windows.Input;
using Core.Settings;
using LauncherFinal.Helper;
using LauncherFinal.Models;
using LauncherFinal.Models.AuthModules;
using LauncherFinal.Models.Settings;
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

            try
            {
                var launcher = new LaunchWorker(_settings, ChoosenServer,
                    SelectedAuthModule, Login, Password);

                var message = await launcher.Launch(OnExit);
                if (!string.IsNullOrEmpty(message))
                    throw new Exception(message);
            }
            catch (Exception e)
            {
                Trace.Write(e);
                await MessageService.ShowMessage(e.Message);
            }
        }

        private async void OnExit(object sender, bool e)
        {
            Application.Current.MainWindow?.Show();

            if (e)
            {
                await MessageService.ShowMessage("Произошла какая-то ошибка");
            }
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
            var choosen = ChoosenServer;

            Servers.Clear();
            SetModules(config);
            PassFromSettings();
            Login = _settings.Login;
            RememberPassword = _settings.SavePass;            

            if (config?.Servers?.Any() == true)
            {
                var servers = config
                    .Servers
                    .Select(x => new ServerViewModel(x));

                Servers = new ObservableCollection<ServerViewModel>(servers);

                if (choosen != null)
                {
                    ChoosenServer = Servers.FirstOrDefault(x => Equals(choosen.Address, x.Address));
                }
                else
                {
                    ChoosenServer = Servers.FirstOrDefault();
                }

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
            if (RememberPassword)
            {
                var password = new PasswordSettings
                {
                    Password = _crypter.GetRandomSalt(),
                    Salt = _crypter.GetRandomSalt()
                };

                password.Encrypted = _crypter.Encrypt<AesManaged>(Password.ConvertToString(),
                    password.Password,
                    password.Salt);

                _settings.Password = password;
            }
            else
            {
                _settings.Password = null;
            }
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
                    modules = new List<ModuleTypes> {config.AuthModuleSettings.Type};
                }
            }

            Modules = modules;
        }

        #endregion
    }
}
