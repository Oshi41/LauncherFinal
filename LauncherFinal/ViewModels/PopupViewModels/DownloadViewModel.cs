using System;
using System.Threading.Tasks;
using System.Windows.Input;
using LauncherFinal.Models;
using Mvvm.Commands;

namespace LauncherFinal.ViewModels.PopupViewModels
{
    /// <summary>
    /// Ориентирована на показ в DialogHost
    /// </summary>
    class DownloadViewModel : PopupHostViewModelBase
    {
        #region Fields

        private readonly bool _autoClose;
        private int _speed;
        private int _percantage;
        private bool _isError;
        private readonly DownloadManager _manager;

        #endregion

        #region Properties

        public string Url { get; }
        public string Filename { get; }
        public ICommand CancelDownloadCommand { get; }

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
            string filename = null,
            bool autoClose = true)
            : base(dialogHostName, autoClose)
        {
            Url = url;
            Filename = filename;
            _autoClose = autoClose;

            _manager = new DownloadManager(url, filename);

            _manager.ProgressChanged += OnProgressChanged;
            _manager.DownloadComplited += OnComplited;

            CancelDownloadCommand = new DelegateCommand(() => _manager.Cancel(), () => _manager.IsDownloading);
        }

        #region Event handlers

        private void OnProgressChanged(object sender, EventArgs e)
        {
            Percantage = _manager.Percantage;
        }

        private void OnComplited(object sender, string e)
        {
            IsError = _manager.IsError;

            if (_autoClose)
                ClosePopup();
        }

        #endregion

        #region Methods

        public async Task<string> Start()
        {
            ShowPopup();

            return await _manager.Download();
        }

        #endregion
    }
}
