using System;
using System.Threading.Tasks;
using LauncherFinal.Models;

namespace LauncherFinal.ViewModels.PopupViewModels
{
    class CompressionViewModel : PopupHostViewModelBase
    {
        private readonly bool _autoClose;
        private readonly CompressionManager _manager;

        public CompressionViewModel(string dialogHostName, 
            string zip, 
            string folder,
            bool autoClose,
            bool deleteZip = false,
            bool clearDestFolder = false) 
            : base(dialogHostName)
        {
            _autoClose = autoClose;
            _manager = new CompressionManager(zip, folder, deleteZip, clearDestFolder);
            _manager.OnComplited += OnComplited;
        }

        private void OnComplited(object sender, EventArgs e)
        {
            if (_autoClose)
            {
                ClosePopup();
            }
        }

        public async Task<bool> Extract()
        {
            ShowPopup();
            return await _manager.Extract();
        }
    }
}
