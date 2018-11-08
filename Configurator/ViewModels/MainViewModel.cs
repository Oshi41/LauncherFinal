using System.IO;
using System.Windows.Input;
using Configurator.Models;
using Configurator.Services;
using Core.Json;
using Microsoft.Win32;
using Mvvm;
using Mvvm.Commands;
using Newtonsoft.Json.Linq;

namespace Configurator.ViewModels
{
    public class MainViewModel : BindableBase
    {
        private string _projectUri;
        private string _updateLink;
        public ProjectViewModel ProjectViewModel { get; } = new ProjectViewModel();
        public UpdateViewModel UpdateViewModel { get; } = new UpdateViewModel();

        public string ProjectUri
        {
            get => _projectUri;
            set => SetProperty(ref _projectUri, value);
        }

        public string UpdateLink
        {
            get { return _updateLink; }
            set { SetProperty(ref _updateLink, value); }
        }

        public ICommand SaveProjConfig { get; private set; }
        public ICommand SaveUpdConfig { get; private set; }
        public ICommand EditProjectCommand { get; private set; }
        public ICommand EditUpdateCommand { get; private set; }

        public MainViewModel()
        {
            SaveProjConfig = new DelegateCommand(() => OnSaveCommand(ProjectViewModel.ToJson()));
            SaveUpdConfig = new DelegateCommand(() => OnSaveCommand(UpdateViewModel.ToJson()));
            EditProjectCommand = new DelegateCommand(OnEditProject);
            EditUpdateCommand = new DelegateCommand(OnEditUpd);
        }

        private void OnEditUpd()
        {
            if (WindowService.ShowHorizontalialog(UpdateViewModel, 300) == true)
            {
                OnSaveCommand(UpdateViewModel.ToJson());
            }
        }

        private void OnEditProject()
        {
            if (WindowService.ShowVerticalDialog(ProjectViewModel, 480) == true)
            {
                OnSaveCommand(ProjectViewModel.ToJson());
            }
        }

        private void OnSaveCommand(JObject obj)
        {
            var dlg = new SaveFileDialog();
            if (dlg.ShowDialog() != true)
                return;

            var path = dlg.FileName;

            File.WriteAllText(path, obj.ToStringIgnoreNull());
        }
    }
}
