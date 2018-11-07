using System.Collections.Generic;
using Mvvm;

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
            get => _hasSite;
            set => SetProperty(ref _hasSite, value);
        }

        public string ProjectSite
        {
            get => _projectSite;
            set => SetProperty(ref _projectSite, value);
        }
    }
}
