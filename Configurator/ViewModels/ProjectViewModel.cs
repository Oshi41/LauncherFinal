using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Configurator.Models;
using Configurator.Services;
using Core.Json;
using Core.Settings;
using Mvvm;
using Mvvm.Commands;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Configurator.ViewModels
{
    public class ProjectViewModel : BindableBase
    {
        private string _projectSite;

        private ObservableCollection<ServerViewModel> _servers = new ObservableCollection<ServerViewModel>();
        private ServerViewModel _current;
        private bool _saveAuth = true;

        public string ProjectSite
        {
            get => _projectSite;
            set => SetProperty(ref _projectSite, value);
        }

        public ObservableCollection<ServerViewModel> Servers
        {
            get => _servers;
            set => SetProperty(ref _servers, value);
        }

        public ServerViewModel Current
        {
            get => _current;
            set => SetProperty(ref _current, value);
        }

        public AuthViewModule Auth { get; } = new AuthViewModule();

        public bool SaveAuth
        {
            get => _saveAuth;
            set => SetProperty(ref _saveAuth, value);
        }

        public ICommand AddServer { get; private set; }
        public ICommand EditServer { get; private set; }
        public ICommand DeleteServer { get; private set; }


        public ProjectViewModel()
        {
            AddServer = new DelegateCommand(OnAddServer);

            EditServer = new DelegateCommand(() => WindowService.ShowVerticalDialog(Current, 500),
                () => Current != null);

            DeleteServer = new DelegateCommand(() => Servers.Remove(Current),
                () => Servers.Contains(Current));
        }

        public ProjectViewModel(AuthViewModule auth)
            : this()
        {
            Auth = auth;
        }

        private void OnAddServer()
        {
            var vm = new ServerViewModel();
            if (WindowService.ShowVerticalDialog(vm, 500) == true)
            {
                Servers.Add(vm);
            }
        }

        public JObject ToJson()
        {
            var serializer = new SettingsSerializer();

            var servers = Servers.Select(x => x.ToJson()).ToList();
            return serializer.WriteProjectConfig(servers, Auth.ToJson(), ProjectSite);
        }
    }
}
