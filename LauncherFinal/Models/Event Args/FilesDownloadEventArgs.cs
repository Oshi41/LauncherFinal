using System;
using System.Linq;

namespace LauncherFinal.Models.Event_Args
{
    public class FilesDownloadEventArgs : EventArgs
    {
        public string File => Files?.FirstOrDefault();

        public string[] Files { get; }

        public FilesDownloadEventArgs(params string[] actual)
        {
            Files = actual;
        }
    }
}
