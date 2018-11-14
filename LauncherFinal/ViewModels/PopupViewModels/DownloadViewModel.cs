using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using LauncherFinal.Models;
using LauncherFinal.Models.Event_Args;
using Mvvm.Commands;

namespace LauncherFinal.ViewModels.PopupViewModels
{
    /// <summary>
    /// Ориентирована на показ в DialogHost
    /// </summary>
    class DownloadViewModel : PopupHostViewModelBase
    {
        #region Fields

        private int _speed;
        private int _percantage;
        private bool _isError;
        private readonly IDownloadManager _manager;

        #endregion

        #region Properties

        public ICommand CancelDownloadCommand { get; private set; }

        public int Speed
        {
            get => _speed;
            set => SetProperty(ref _speed, value);
        }

        public int Percantage
        {
            get => _percantage;
            set => SetProperty(ref _percantage, value);
        }

        public bool IsError
        {
            get => _isError;
            set => SetProperty(ref _isError, value);
        }

        #endregion

        public DownloadViewModel(string dialogHostName,
            string url,
            string filename = null)
            : base(dialogHostName, true)
        {
            _manager = new DownloadManager(url, filename);
            BaseInit();
        }

        public DownloadViewModel(string dialogHostName,
            List<string> urls)
            : base(dialogHostName, true)
        {
            _manager = new MultiDownloader(urls);
            BaseInit();
        }

        private void BaseInit()
        {
            _manager.ProgressChanged += OnProgressChanged;
            _manager.DownloadComplited += OnComplited;

            CancelDownloadCommand = new DelegateCommand(() => _manager.Cancel(), () => _manager.IsDownloading);
        }

        #region Event handlers

        private void OnProgressChanged(object sender, int i)
        {
            Percantage = i;
        }

        private void OnComplited(object sender, FilesDownloadEventArgs filesDownloadEventArgs)
        {
            IsError = _manager.IsError;

            ClosePopup();
        }

        #endregion

        #region Methods

        public async Task<FilesList> Start()
        {
            ShowPopup();

            return await _manager.Download();
        }

        #endregion
    }
}
