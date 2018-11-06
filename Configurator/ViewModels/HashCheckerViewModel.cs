using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
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
            get { return _root; }
            set { SetProperty(ref _root, value); }
        }

        public ObservableCollection<HashItemViewModel> Hashes
        {
            get { return _hashes; }
            set { SetProperty(ref _hashes, value); }
        }

        public HashItemViewModel Selected
        {
            get { return _selected; }
            set { SetProperty(ref _selected, value); }
        }

        public ICommand CalculateCommand { get; private set; }
        public ICommand DeleteCommand { get; private set; }
        public ICommand SelectRootCommand { get; private set; }


        #endregion

        public HashCheckerViewModel()
        {
            CalculateCommand = new DelegateCommand(OnCalculateCommand, OnCanCalculateCommand);
            DeleteCommand = new DelegateCommand(() => Hashes.Remove(Selected),
                () => Selected != null && Hashes.Contains(Selected));
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

        private bool OnCanCalculateCommand()
        {
            return Directory.Exists(Root);
        }

        private async void OnCalculateCommand()
        {
            var dlg = new FolderBrowserDialog
            {
                SelectedPath = Root,
                ShowNewFolderButton = false,
            };

            if (dlg.ShowDialog() != DialogResult.OK)
                return;

            if (!Uri.TryCreate(Root, UriKind.Absolute, out var baseUri))
                return;

            if (!Uri.TryCreate(dlg.SelectedPath, UriKind.Absolute, out var selected))
                return;

            string path = null;

            try
            {
                path = _pathWorker.RelativePath(Root, dlg.SelectedPath);
            }
            catch (Exception e)
            {
                Trace.Write(e);
                
                // todo Make notification to user
            }

            var hash = await Task.Run(() => _checker.CreateMd5ForFolder(dlg.SelectedPath));

            if (Selected == null)
            {
                var vm = new HashItemViewModel(path, hash);
                Hashes.Add(vm);
                Selected = vm;
            }
            else
            {
                Selected.Hash = hash;
                Selected.Path = path;
            }
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
            get { return _path; }
            set { SetProperty(ref _path, value); }
        }

        public string Hash
        {
            get { return _hash; }
            set { SetProperty(ref _hash, value); }
        }

        public HashItemViewModel(string path, string hash)
        {
            _path = path;
            _hash = hash;
        }
        
    }
}
