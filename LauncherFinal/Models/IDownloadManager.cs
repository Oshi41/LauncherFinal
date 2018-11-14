using System;
using System.Threading.Tasks;
using LauncherFinal.Models.Event_Args;

namespace LauncherFinal.Models
{
    public interface IDownloadManager : IDisposable
    {
        bool IsDownloading { get; }
        bool IsError { get;  }
        Exception LastError { get; }
        int Percantage { get; }
        int Speed { get; }

        event EventHandler<FilesDownloadEventArgs> DownloadComplited;
        event EventHandler DownloadStarted;
        event EventHandler<int> ProgressChanged;

        void Cancel();
        bool Clear();

        Task<FilesList> Download();
    }
}