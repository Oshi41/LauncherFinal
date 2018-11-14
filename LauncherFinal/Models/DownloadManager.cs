using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Core.Models;
using LauncherFinal.Models.Event_Args;

namespace LauncherFinal.Models
{
    /// <summary>
    ///     Класс наблюдающий за скоростью скачивания.
    ///     <para>
    ///         Использование: при скачивании файла клиентом подписываемся на DownloadProgresChanged.
    ///         Передаём туда кол-во загруженных байт. Арбитр рассчитывает кол-во полученных бит за
    ///         заданное время
    ///     </para>
    /// </summary>
    public class DownloadManager : IDownloadManager
    {
        #region Fields
        private readonly DirectWebClient _client = new DirectWebClient();
        private readonly string _uri;
        private readonly string _file;
        private readonly int _interval;

        private long _previouse;
        private long _current;
        private DateTime _prevData;
        private int _percantage;

        #endregion

        /// <summary>
        /// </summary>
        /// <param name="file">Куда скачиваем</param>
        /// <param name="uri">Что скачиваем</param>
        /// <param name="interval">Интервал замера скорости скачивания</param>
        public DownloadManager(string uri, string file = null, int interval = 40)
        {
            _uri = uri;
            _file = file ?? Path.GetTempFileName();

            if (File.Exists(_file))
                File.Delete(_file);

            _interval = interval;

            // если интервал задан меньше 0, отключаем проверку скорости
            if (_interval > 0)
                _client.DownloadProgressChanged += OnProgressChanged;

            _client.DownloadFileCompleted += OnDownloadComplited;
        }

        #region Properties

        /// <summary>
        /// Скорость скачивания
        /// </summary>
        public int Speed { get; private set; }

        /// <summary>
        /// Процентаж выполнения
        /// </summary>
        public int Percantage
        {
            get => _percantage;
            private set
            {
                if (_percantage != value) return;

                _percantage = value;
                ProgressChanged?.Invoke(this, Percantage);
            }
        }

        public bool IsDownloading => _client.IsBusy;

        public bool IsError { get; set; }

        public Exception LastError { get; private set; }

        #endregion

        #region Events

        public event EventHandler<FilesDownloadEventArgs> DownloadComplited;
        public event EventHandler DownloadStarted;
        public event EventHandler<int> ProgressChanged;

        #endregion

        #region Puclic methods

        /// <summary>
        /// Скачивает файл и возвращает его путь
        /// </summary>
        /// <returns></returns>
        public async Task<string> Download()
        {
            _prevData = DateTime.Now;

            try
            {
                var task = _client.DownloadFileAsync(_uri, _file);
                DownloadStarted?.Invoke(this, EventArgs.Empty);

                await task;
            }
            catch (Exception e)
            {
                Trace.Write(e);
                LastError = e;
                DownloadComplited?.Invoke(this, new FilesDownloadEventArgs());
            }

            return _file;
        }

        public void Cancel()
        {
            if (IsDownloading)
                _client.CancelAsync();

        }

        public bool Clear()
        {
            try
            {
                if (!File.Exists(_file))
                    throw new FileNotFoundException($"Can't find file - {_file}");

                File.Delete(_file);
                return true;
            }
            catch (Exception e)
            {
                Trace.Write(e);
                LastError = e;
                return false;
            }
        }

        #endregion

        #region Methods

        private void OnProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            _current = e.BytesReceived;
            Percantage = e.ProgressPercentage;

            var now = DateTime.Now;
            var mls = (now - _prevData).TotalMilliseconds;

            if (mls < _interval)
                return;

            _prevData = now;

            var bytes = _current - _previouse;
            _previouse = _current;

            var kbInSec = bytes / mls * 1024.0 / 1000;

            Speed = (int)Math.Floor(kbInSec);
            ProgressChanged?.Invoke(this, Percantage);
        }

        private void OnDownloadComplited(object sender, AsyncCompletedEventArgs e)
        {
            IsError = e.Cancelled || e.Error != null;
            LastError = e.Error;

            DownloadComplited?.Invoke(sender, new FilesDownloadEventArgs(_file));
        }

        #endregion

        public void Dispose()
        {
            _client?.Dispose();
            Clear();
        }

        async Task IDownloadManager.Download()
        {
            await Download();
        }
    }
}
