using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LauncherCore.Settings;
using Mvvm;
using Newtonsoft.Json.Linq;

namespace Configurator.ViewModels
{
    class ProjectViewModel : BindableBase
    {
        private bool _hasSite;
        private string _projectSite;
        private AuthViewModule _auth;
        private List<ServerViewModel> servers;

        public bool HasSite
        {
            get { return _hasSite; }
            set { SetProperty(ref _hasSite, value); }
        }

        public string ProjectSite
        {
            get { return _projectSite; }
            set { SetProperty(ref _projectSite, value); }
        }
    }
}
