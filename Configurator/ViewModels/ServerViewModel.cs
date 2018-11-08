using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Configurator.Services;
using Core.Json;
using Mvvm;
using Mvvm.Commands;
using Newtonsoft.Json.Linq;

namespace Configurator.ViewModels
{
    public class ServerViewModel : BindableBase
    {
        private string _name;
        private string _clientUri;
        private string _address;

        private HashCheckerViewModel _hashCheckerViewModel = new HashCheckerViewModel();
        private bool _saveHashes = true;

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

        public string Address
        {
            get => _address;
            set => SetProperty(ref _address, value);
        }

        public bool SaveHashes
        {
            get => _saveHashes;
            set => SetProperty(ref _saveHashes, value);
        }

        public HashCheckerViewModel HashCheckerViewModel
        {
            get => _hashCheckerViewModel;
            set => SetProperty(ref _hashCheckerViewModel, value);
        }

        public ICommand EditHashes { get; private set; }

        public ServerViewModel()
        {
            EditHashes = new DelegateCommand(OnEditHash);
        }

        public ServerViewModel(Dictionary<string, string> hashes)
            : this()
        {
            _hashCheckerViewModel = new HashCheckerViewModel(hashes);
        }

        private void OnEditHash()
        {
            var vm = new HashCheckerViewModel(HashCheckerViewModel);
            if (WindowService.ShowVerticalDialog(vm, 420) == true)
            {
                HashCheckerViewModel = vm;
            }
        }

        public JObject ToJson()
        {
            var serializer = new SettingsSerializer();

            var hashes = SaveHashes
                ? HashCheckerViewModel.Hashes.ToDictionary(x => x.Path, x => x.Hash)
                : null;

            return serializer.WriteServer(Name, Address, ClientUri, hashes);
        }
    }
}
