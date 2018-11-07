using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Configurator.Models;
using Configurator.Services;
using Core.Settings;
using Mvvm;
using Mvvm.Commands;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Configurator.ViewModels
{
    public class ProjectViewModel : BindableBase
    {
        private readonly Pinger _pinger = new Pinger();

        private bool? _siteChecked;
        private string _projectSite;

        private AuthViewModule _auth = new AuthViewModule();
        private ObservableCollection<ServerViewModel> _servers = new ObservableCollection<ServerViewModel>();
        private ServerViewModel _current;
        private bool _saveAuth = true;

        public bool? SiteChecked
        {
            get => _siteChecked;
            set => SetProperty(ref _siteChecked, value);
        }

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

        public AuthViewModule Auth
        {
            get => _auth;
            private set => SetProperty(ref _auth, value);
        }

        public bool SaveAuth
        {
            get => _saveAuth;
            set => SetProperty(ref _saveAuth, value);
        }

        public ICommand AddServer { get; private set; }
        public ICommand EditServer { get; private set; }
        public ICommand DeleteServer { get; private set; }
        public ICommand EditAuthSettings { get; private set; }
        public ICommand CheckSite { get; private set; }


        public ProjectViewModel()
        {
            EditAuthSettings = new DelegateCommand(OnEditAuth);

            AddServer = new DelegateCommand(OnAddServer);

            EditServer = new DelegateCommand(() => WindowService.ShowDialog(Current, 420),
                () => Current != null);

            DeleteServer = new DelegateCommand(() => Servers.Remove(Current),
                () => Servers.Contains(Current));

            CheckSite = new DelegateCommand(async () => SiteChecked = await _pinger.CheckPing(ProjectSite),
                () => _pinger.CanPing(ProjectSite));
        }

        public ProjectViewModel(AuthViewModule auth)
            : this()
        {
            Auth = auth;
        }

        private void OnAddServer()
        {
            var vm = new ServerViewModel();
            if (WindowService.ShowDialog(vm, 420) == true)
            {
                Servers.Add(vm);
            }
        }

        private void OnEditAuth()
        {
            var vm = new AuthViewModule();
            if (WindowService.ShowDialog(vm, 400) == true)
            {
                Auth = vm;
            }
        }

        public JObject ToJson()
        {
            IProjectConfig conf;
            var obj = new JObject();

            var serializer = JsonSerializer.CreateDefault();
            var writer = obj.CreateWriter();

            if (!string.IsNullOrWhiteSpace(ProjectSite))
            {
                writer.WritePropertyName(nameof(conf.ProjectSite));
                writer.WriteValue(ProjectSite);
            }

            if (Servers.Any())
            {
                writer.WritePropertyName(nameof(conf.Servers));
                writer.WriteStartArray();

                foreach (var server in Servers)
                {
                    serializer.Serialize(writer, server.ToJson());
                }

                writer.WriteEndArray();
            }

            writer.WritePropertyName(nameof(conf.AuthModuleSettings));
            serializer.Serialize(writer, Auth?.ToJson());

            writer.Close();

            return obj;
        }
    }
}
