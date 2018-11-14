using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        private ObservableCollection<string> _clientUries = new ObservableCollection<string>();
        private string _address;

        private HashCheckerViewModel _hashCheckerViewModel = new HashCheckerViewModel();
        private bool _saveHashes = true;
        private string _selected;

        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        public ObservableCollection<string> ClientUries
        {
            get => _clientUries;
            set => SetProperty(ref _clientUries, value);
        }

        public string Selected
        {
            get => _selected;
            set => SetProperty(ref _selected, value);
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
        public ICommand AddUrl { get; private set; }
        public ICommand DeleteUrl { get; private set; }

        public ServerViewModel()
        {
            EditHashes = new DelegateCommand(OnEditHash);
            AddUrl = new DelegateCommand(() => ClientUries.Add(" "));
            DeleteUrl = new DelegateCommand(() => ClientUries.Remove(Selected), () => Selected != null && ClientUries.Contains(Selected));
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

            return serializer.WriteServer(Name, Address, ClientUries, hashes);
        }
    }
}
