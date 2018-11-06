using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using Configurator.Models;
using LauncherCore.Settings;
using Mvvm;
using Mvvm.Commands;

namespace Configurator.ViewModels
{
    class AuthViewModule : BindableBase
    {
        private readonly Pinger _pinger = new Pinger();
        private ModuleTypes _module;
        private string _uri;
        private bool? _checked;

        public ModuleTypes Custom => ModuleTypes.Custom;
        public ModuleTypes None => ModuleTypes.None;

        public List<ModuleTypes> DefaultModules { get;  }

        public ModuleTypes Module
        {
            get { return _module; }
            set { SetProperty(ref _module, value); }
        }

        public string Uri
        {
            get { return _uri; }
            set { SetProperty(ref _uri, value); }
        }

        public bool? Checked
        {
            get { return _checked; }
            set { SetProperty(ref _checked, value); }
        }

        public ICommand PingCommand { get; private set; }

        public AuthViewModule()
        {
            PingCommand = new DelegateCommand(async () => Checked = await _pinger.CheckPing(Uri),
                () => !_pinger.CanPing(Uri));

            DefaultModules = Enum.GetValues(typeof(ModuleTypes))
                .OfType<ModuleTypes>()
                .Except(new[] {Custom, None})
                .ToList();
        }
    }
}

