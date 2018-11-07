using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Configurator.Models;
using Configurator.Services;
using Core.Models;
using Core.Settings;
using Mvvm;
using Mvvm.Commands;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Configurator.ViewModels
{
    public class ServerViewModel : BindableBase
    {
        private readonly Pinger _pinger = new Pinger();

        private string _name;
        private string _clientUri;
        private string _address;
        private bool? _isClientChecked;
        private bool? _isServerUp;

        private bool _isCheckingServer;
        private HashCheckerViewModel _hashCheckerViewModel = new HashCheckerViewModel();
        private bool _saveHashes = true;

        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        public string ClientUri
        {
            get => _clientUri;
            set => SetProperty(ref _clientUri, value);
        }

        public bool? IsClientChecked
        {
            get => _isClientChecked;
            set => SetProperty(ref _isClientChecked, value);
        }

        public string Address
        {
            get => _address;
            set => SetProperty(ref _address, value);
        }

        public bool? IsServerUp
        {
            get => _isServerUp;
            set => SetProperty(ref _isServerUp, value);
        }

        public bool SaveHashes
        {
            get => _saveHashes;
            set => SetProperty(ref _saveHashes, value);
        }

        public HashCheckerViewModel HashCheckerViewModel
        {
            get => _hashCheckerViewModel;
            set => SetProperty(ref _hashCheckerViewModel, value);
        }

        public ICommand PingFiles { get; private set; }
        public ICommand CheckServer { get; private set; }
        public ICommand EditHashes { get; private set; }


        public ServerViewModel()
        {
            PingFiles = new DelegateCommand(async () => IsClientChecked = await _pinger.CheckPing(ClientUri),
                () => _pinger.CanPing(ClientUri));

            CheckServer = new DelegateCommand(OnCheckServer, OnCanCheckServer);

            EditHashes = new DelegateCommand(OnEditHash);
        }

        private bool OnCanCheckServer()
        {
            return !_isCheckingServer && !string.IsNullOrWhiteSpace(Address);
        }

        private async void OnCheckServer()
        {
            _isCheckingServer = true;

            var stat = new ServerStat(Address);
            await stat.GetInfoAsync();
            IsServerUp = stat.ServerUp;

            _isCheckingServer = false;
        }

        public ServerViewModel(Dictionary<string, string> hashes)
            : this()
        {
            _hashCheckerViewModel = new HashCheckerViewModel(hashes);
        }

        private void OnEditHash()
        {
            var vm = new HashCheckerViewModel(HashCheckerViewModel);
            if (WindowService.ShowDialog(vm, 420) == true)
            {
                HashCheckerViewModel = vm;
            }
        }

        public JObject ToJson()
        {
            IServer server;

            var obj = new JObject();

            var writer = obj.CreateWriter();

            writer.WritePropertyName(nameof(server.Address));
            writer.WriteValue(Address);

            writer.WritePropertyName(nameof(server.Name));
            writer.WriteValue(Name);

            writer.WritePropertyName(nameof(server.DownloadLink));
            writer.WriteValue(ClientUri);

            if (SaveHashes && HashCheckerViewModel?.Hashes?.Any() == true)
            {
                writer.WritePropertyName(nameof(server.DirHashCheck));
                var serializer = JsonSerializer.CreateDefault();
                serializer.Serialize(writer, HashCheckerViewModel.Hashes.ToDictionary(x => x.Path, x => x.Hash));
            }

            writer.Close();

            return obj;
        }
    }
}
