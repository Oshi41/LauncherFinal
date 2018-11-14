using System;
using System.Collections.Generic;
using System.Linq;

namespace LauncherFinal.Models.Event_Args
{
    public class FilesDownloadEventArgs : EventArgs
    {
        public FilesList Files { get; }

        public FilesDownloadEventArgs(params string[] actual)
        {
            Files = new FilesList(actual);
        }
    }

    public class FilesList
    {
        public string File => Files?.FirstOrDefault();

        public List<string> Files { get; }

        public FilesList(IEnumerable<string> files)
        {
            Files = files?.ToList() ?? new List<string>();
        }

        public FilesList(string path)
            : this(new []{path}) 
        {  }
    }
}
