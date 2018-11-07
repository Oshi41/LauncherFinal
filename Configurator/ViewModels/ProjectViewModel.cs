using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Configurator.Services;
using Core.Settings;
using Mvvm;
using Mvvm.Commands;
using Newtonsoft.Json.Linq;

namespace Configurator.ViewModels
{
    class ProjectViewModel : BindableBase
    {
        private bool _siteChecked;
        private string _projectSite;

        private AuthViewModule _auth;
        private ObservableCollection<ServerViewModel> _servers = new ObservableCollection<ServerViewModel>();
        private ServerViewModel _current;

        public bool SiteChecked
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

        public ICommand AddServer { get; private set; }
        public ICommand EditServer { get; private set; }
        public ICommand DeleteServer { get; private set; }
        public ICommand EditAuthSettings { get; private set; }

        public ProjectViewModel()
        {
            EditAuthSettings = new DelegateCommand(OnEditAuth);

            AddServer = new DelegateCommand(OnAddServer);

            EditServer = new DelegateCommand(() => WindowService.ShowDialog(Current, 420), 
                () => Current != null);

            DeleteServer = new DelegateCommand(() => Servers.Remove(Current),
                () => Servers.Contains(Current));
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
                _auth = vm;
            }
        }

        private JObject ToJson()
        {
            IProjectConfig conf;
            var obj = new JObject();

            var writer = obj.CreateWriter();

            writer.WritePropertyName(nameof(conf.ProjectSite));
            writer.WriteValue(ProjectSite);

            if (_auth != null)
            {
                writer.WritePropertyName(nameof(conf.AuthModuleSettings));
                writer.WriteValue((_auth.ToJson()));
            }

            if (Servers.Any())
            {
                writer.WritePropertyName(nameof(conf.Servers));
                writer.WriteValue(new JObject(Servers.Select(x => x.ToJson())));
            }

            writer.Close();

            return obj;
        }
    }
}
