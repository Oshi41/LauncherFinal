using System.Windows.Input;
using Configurator.Models;
using Core.Json;
using Core.Settings;
using Mvvm;
using Mvvm.Commands;
using Newtonsoft.Json.Linq;

namespace Configurator.ViewModels
{
    public class AuthViewModule : BindableBase
    {
        private readonly Pinger _pinger = new Pinger();
        private ModuleTypes _module = ModuleTypes.None;
        private string _uri;
        private bool? _checked;
        private bool _strictUsage;

        public ModuleTypes Module
        {
            get => _module;
            set => SetProperty(ref _module, value);
        }

        public string Uri
        {
            get => _uri;
            set => SetProperty(ref _uri, value);
        }

        public bool? Checked
        {
            get => _checked;
            set => SetProperty(ref _checked, value);
        }

        public bool StrictUsage
        {
            get => _strictUsage;
            set => SetProperty(ref _strictUsage, value);
        }

        public ICommand PingCommand { get; private set; }

        public AuthViewModule()
        {
            PingCommand = new DelegateCommand(async () => Checked = await _pinger.CheckPing(Uri),
                () => _pinger.CanPing(Uri));
        }

        public JObject ToJson()
        {
            var serializer = new SettingsSerializer();
            return serializer.WriteAuth(Uri, Module, StrictUsage);
        }
    }
}

