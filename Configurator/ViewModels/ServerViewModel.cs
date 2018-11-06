using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Configurator.Models;
using LauncherCore.Models;
using Mvvm;
using Mvvm.Commands;

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

        public BindableBase HashViewModel { get; } = new HashCheckerViewModel();

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
            //todo shoe dialog
        }
    }
}
