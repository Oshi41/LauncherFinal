using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Configurator.Models;
using Core.Json;
using Mvvm;
using Mvvm.Commands;
using Newtonsoft.Json.Linq;

namespace Configurator.ViewModels
{
    public class UpdateViewModel : BindableBase
    {
        private readonly Pinger _pinger = new Pinger();
        private string _uri;
        private Version _version;
        private bool? _corectUri;

        public string Uri
        {
            get => _uri;
            set => SetProperty(ref _uri, value);
        }

        public Version Version
        {
            get => _version;
            set => SetProperty(ref _version, value);
        }

        public bool? CorectUri
        {
            get => _corectUri;
            set => SetProperty(ref _corectUri, value);
        }

        public ICommand PingUri { get; private set; }

        public UpdateViewModel()
        {
            PingUri = new DelegateCommand(async () => CorectUri = await _pinger.CheckPing(Uri),
                () => _pinger.CanPing(Uri));
        }

        public JObject ToJson()
        {
            var ser = new SettingsSerializer();
            return ser.WriteUpdateConfig(Uri, Version);
        }
    }
}
