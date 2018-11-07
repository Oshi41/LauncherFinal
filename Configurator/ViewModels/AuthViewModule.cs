using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Configurator.Models;
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

        public ModuleTypes Custom => ModuleTypes.Custom;
        public ModuleTypes None => ModuleTypes.None;

        public List<ModuleTypes> DefaultModules { get; }

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
                () => !_pinger.CanPing(Uri));

            DefaultModules = Enum.GetValues(typeof(ModuleTypes))
                .OfType<ModuleTypes>()
                .Except(new[] { Custom, None })
                .ToList();
        }

        public JObject ToJson()
        {
            IAuthModuleSettings set;

            var obj = new JObject();

            var writer = obj.CreateWriter();

            writer.WritePropertyName(nameof(set.AuthUri));
            writer.WriteValue(Uri);

            writer.WritePropertyName(nameof(set.StrictUsage));
            writer.WriteValue(StrictUsage);

            writer.WritePropertyName(nameof(set.Type));
            writer.WriteValue(Module);

            writer.Close();

            return obj;
        }
    }
}

