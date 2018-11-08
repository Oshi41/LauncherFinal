using System.IO;
using System.Windows.Input;
using Configurator.Models;
using Configurator.Services;
using Core.Json;
using Microsoft.Win32;
using Mvvm;
using Mvvm.Commands;

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

        public ICommand SaveCommand { get; private set; }
        public ICommand EditProjectCommand { get; private set; }
        public ICommand EditUpdateCommand { get; private set; }

        public MainViewModel()
        {
            SaveCommand = new DelegateCommand(OnSaveCommand);
            EditProjectCommand = new DelegateCommand(() => WindowService.ShowVerticalDialog(ProjectViewModel, 480));
            EditUpdateCommand = new DelegateCommand(() => WindowService.ShowHorizontalialog(UpdateViewModel, 300));
        }

        private void OnSaveCommand()
        {
            var dlg = new SaveFileDialog();
            if (dlg.ShowDialog() != true)
                return;

            var path = dlg.FileName;
            var serializer = new SettingsSerializer();
            var settings = serializer.WriteSettings(UpdateViewModel.ToJson(), UpdateLink, ProjectViewModel.ToJson(), ProjectUri);

            File.WriteAllText(path, settings.ToStringIgnoreNull());
        }
    }
}
