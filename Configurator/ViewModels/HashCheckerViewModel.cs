using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
using Configurator.Models;
using LauncherCore.Models;
using Mvvm;
using Mvvm.Commands;

namespace Configurator.ViewModels
{
    class HashCheckerViewModel : BindableBase
    {
        #region Fields

        private readonly PathWorker _pathWorker = new PathWorker();
        private readonly HashChecker _checker = new HashChecker();

        private string _root;
        private ObservableCollection<HashItemViewModel> _hashes = new ObservableCollection<HashItemViewModel>();
        private HashItemViewModel _selected;

        #endregion

        #region Properties

        public string Root
        {
            get => _root;
            set => SetProperty(ref _root, value);
        }

        public ObservableCollection<HashItemViewModel> Hashes
        {
            get => _hashes;
            set => SetProperty(ref _hashes, value);
        }

        public HashItemViewModel Selected
        {
            get => _selected;
            set => SetProperty(ref _selected, value);
        }

        public ICommand AddCommand { get; private set; }
        public ICommand EditCommand { get; private set; }
        public ICommand DeleteCommand { get; private set; }
        public ICommand SelectRootCommand { get; private set; }


        #endregion

        public HashCheckerViewModel()
        {
            AddCommand = new DelegateCommand(async () =>
            {
                var vm = await CreateHash();
                if (vm != null)
                    Hashes.Add(vm);

            }, () => Directory.Exists(Root));

            DeleteCommand = new DelegateCommand(() => Hashes.Remove(Selected),
                () => Selected != null && Hashes.Contains(Selected));

            EditCommand = new DelegateCommand(async () =>
            {
                var vm = await CreateHash();
                if (vm == null)
                    return;

                Hashes.Remove(Selected);
                Hashes.Add(vm);
                Selected = vm;
            }, () => Selected != null);

            SelectRootCommand = new DelegateCommand(OnSelectRootCommand);

        }

        public HashCheckerViewModel(Dictionary<string, string> hashes)
            : this()
        {
            if (hashes == null || !hashes.Any())
                return;

            var list = hashes.Select(x => new HashItemViewModel(x.Key, x.Value));
            Hashes = new ObservableCollection<HashItemViewModel>(list);
        }

        #region Command Handlers

        private async Task<HashItemViewModel> CreateHash()
        {
            var dlg = new FolderBrowserDialog
            {
                SelectedPath = Root,
                ShowNewFolderButton = false,
            };

            if (dlg.ShowDialog() != DialogResult.OK)
                return null;

            if (!Uri.TryCreate(Root, UriKind.Absolute, out var baseUri))
                return null;

            if (!Uri.TryCreate(dlg.SelectedPath, UriKind.Absolute, out var selected))
                return null;

            string path = null;

            try
            {
                path = _pathWorker.RelativePath(Root, dlg.SelectedPath);
            }
            catch (Exception e)
            {
                Trace.Write(e);

                // todo Make notification to user
                return null;
            }

            var hash = await Task.Run(() => _checker.CreateMd5ForFolder(dlg.SelectedPath));
            return new HashItemViewModel(path, hash);

        }

        private void OnSelectRootCommand()
        {
            var dlg = new FolderBrowserDialog
            {
                SelectedPath = string.IsNullOrWhiteSpace(Root) 
                    ? Root 
                    :  Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                ShowNewFolderButton = false,
            };

            if (dlg.ShowDialog() != DialogResult.OK)
                return;

            Root = dlg.SelectedPath;
        }

        #endregion
    }

    class HashItemViewModel : BindableBase
    {
        private string _path;
        private string _hash;

        public string Path
        {
            get => _path;
            set => SetProperty(ref _path, value);
        }

        public string Hash
        {
            get => _hash;
            set => SetProperty(ref _hash, value);
        }

        public HashItemViewModel(string path, string hash)
        {
            _path = path;
            _hash = hash;
        }
        
    }
}
