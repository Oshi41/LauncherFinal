using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Configurator.Models;
using Configurator.Services;
using LauncherCore.Models;
using LauncherCore.Settings;
using Mvvm;
using Mvvm.Commands;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Configurator.ViewModels
{
    class ServerViewModel : BindableBase
    {
        private readonly Pinger _pinger = new Pinger();

        private readonly ActionArbiter _actionArbiter = new ActionArbiter();
        private string _name;
        private string _clientUri;
        private string _address;
        private bool? _isClientChecked;
        private bool? _isServerUp;

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

        private Dictionary<string, string> _hashes;

        public ICommand PingFiles { get; private set; }
        public ICommand CheckServer { get; private set; }
        public ICommand EditHashes { get; private set; }


        public ServerViewModel()
        {
            PingFiles = new DelegateCommand(async () => IsClientChecked = await _pinger.CheckPing(ClientUri),
                () => _pinger.CanPing(ClientUri));

            CheckServer = new DelegateCommand(() => _actionArbiter.Do(async () =>
            {
                var stat = new ServerStat(Address);
                await stat.GetInfoAsync();
                IsServerUp = stat.ServerUp;
            }), () => !_actionArbiter.IsExecuting );

            EditHashes = new DelegateCommand(OnEditHash);
        }

        private void OnEditHash()
        {
            var y = 420;

            var vm = new HashCheckerViewModel(_hashes);
            if (WindowService.ShowDialog(vm, new Size(y * 1.25, y)) == true)
                _hashes = vm.Hashes.ToDictionary(x => x.Path, x => x.Hash);
        }

        public string ToJson()
        {
            IServer server;

            var obj = new JObject();

            var writer = obj.CreateWriter();

            writer.WritePropertyName(nameof(server.Address));
            writer.WriteValue(Address);

            writer.WritePropertyName(nameof(server.DownloadLink));
            writer.WriteValue(ClientUri);

            writer.WritePropertyName(nameof(server.Name));
            writer.WriteValue(Name);

            writer.WritePropertyName(nameof(server.DirHashCheck));
            writer.WriteValue(_hashes);

            writer.Close();

            return obj.ToString(Formatting.Indented);
        }
    }
}
