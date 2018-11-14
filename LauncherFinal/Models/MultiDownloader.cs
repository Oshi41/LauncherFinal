using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using LauncherFinal.Helper;
using LauncherFinal.Models.Event_Args;

namespace LauncherFinal.Models
{
    public class MultiDownloader : IDownloadManager
    {
        private readonly int _interval;
        private readonly Dictionary<string, string> _map = new Dictionary<string, string>();
        private readonly List<DownloadManager> _managers = new List<DownloadManager>();

        private DateTime _prevData;

        #region Properties


        public bool IsDownloading => _managers.Any(x => x.IsDownloading);

        public bool IsError => _managers.Any(x => x.IsError);

        public Exception LastError => IsError
            ? _managers.FirstOrDefault(x => x.LastError != null)?.LastError
            : null;

        public int Percantage => (int) (_managers.IsNullOrEmpty()
            ? _managers.Average(x => x.Percantage)
            : 0);

        public int Speed => (int) (_managers.IsNullOrEmpty()
            ? _managers.Average(x => x.Speed)
            : 0);

        #endregion

        public MultiDownloader(List<string> urls, int interval = 40)
        {
            _interval = interval;
            if (urls.IsNullOrEmpty())
                return;

            foreach (var url in urls)
            {
                _map.Add(url, Path.GetTempFileName());
            }
        }

        #region Events

        public event EventHandler<FilesDownloadEventArgs> DownloadComplited;
        public event EventHandler DownloadStarted;
        public event EventHandler<int> ProgressChanged;

        #endregion

        #region Methods

        public void Cancel()
        {
            foreach (var manager in _managers)
            {
                manager.Cancel();
            }
        }

        public bool Clear()
        {
            return _managers.All(x => x.Clear());
        }

        public void Dispose()
        {
            _managers.ForEach(x => x.Dispose());
            _managers.Clear();
            _map.Clear();
        }

        public async Task<List<string>> Download()
        {
            DownloadStarted?.Invoke(this, EventArgs.Empty);

            var tasks = _map.Select(x => CreateTask(x.Key, x.Value)).ToList();

            await Task.WhenAll(tasks.ToArray());

            var result = tasks.Select(x => x.Result).ToList();

            DownloadComplited?.Invoke(this, new FilesDownloadEventArgs(result.ToArray()));

            return result;
        }

        #endregion

        private Task<string> CreateTask(string uri, string file)
        {
            var manager = new DownloadManager(uri, file);
            manager.DownloadComplited += OnComplited;
            manager.ProgressChanged += OnProgressChanged;
            _managers.Add(manager);
            return manager.Download();
        }

        private void OnProgressChanged(object sender, int i)
        {
            var now = DateTime.Now;
            var mls = (now - _prevData).TotalMilliseconds;

            if (mls < _interval)
                return;

            ProgressChanged?.Invoke(this, Percantage);
        }

        private void OnComplited(object sender, EventArgs e)
        {
            
        }

        async Task IDownloadManager.Download()
        {
            await Download();
        }
    }
}
