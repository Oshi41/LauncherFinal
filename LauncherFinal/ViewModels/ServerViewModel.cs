using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Models;
using Core.Settings;
using Mvvm;

namespace LauncherFinal.ViewModels
{
    public class ServerViewModel : BindableBase, IServer
    {
        private ServerStat _stateInfo;

        #region Properties

        public string Address { get; }
        public string Name { get; }
        public List<string> DownloadLink { get; }
        public IDictionary<string, string> DirHashCheck { get; }

        public ServerStat StateInfo
        {
            get => _stateInfo;
            set => SetProperty(ref _stateInfo, value);
        }

        #endregion

        public ServerViewModel(IServer server)
        {
            Address = server.Address;
            Name = server.Name;
            DownloadLink = server.DownloadLink;
            DirHashCheck = server.DirHashCheck;

            _stateInfo = new ServerStat(Address);
        }

        public async Task Ping()
        {
            await StateInfo.GetInfoAsync();
            OnPropertyChanged(nameof(StateInfo));
        }
    }
}
