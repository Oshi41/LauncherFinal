using System;
using Core.Json;
using Mvvm;
using Newtonsoft.Json.Linq;

namespace Configurator.ViewModels
{
    public class UpdateViewModel : BindableBase
    {
        private string _uri;
        private Version _version;

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

        public JObject ToJson()
        {
            var ser = new SettingsSerializer();
            return ser.WriteUpdateConfig(Uri, Version);
        }
    }
}
