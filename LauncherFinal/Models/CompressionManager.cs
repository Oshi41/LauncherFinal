using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;

namespace LauncherFinal.Models
{
    class CompressionManager
    {
        #region Fields

        private readonly string _zipName;
        private readonly string _folderName;
        private readonly bool _removeAfterUnzip;
        private readonly bool _cleanExtractingFolder;

        #endregion

        #region Properties

        public Exception LastError { get; private set; }

        public event EventHandler OnComplited;

        #endregion

        public CompressionManager(string zipName,
            string folderName,
            bool removeAfterUnzip = false,
            bool cleanExtractingFolder = false)
        {
            _zipName = zipName;
            _folderName = folderName;
            _removeAfterUnzip = removeAfterUnzip;
            _cleanExtractingFolder = cleanExtractingFolder;

            if (!File.Exists(zipName))
                throw new FileNotFoundException(zipName);

            if (!Directory.Exists(folderName))
            {
                Directory.CreateDirectory(folderName);
            }
        }

        #region Methods

        public async Task<bool> Extract()
        {
            LastError = null;

            try
            {
                await ExtractInner(_removeAfterUnzip, _cleanExtractingFolder);
                return true;
            }
            catch (Exception e)
            {
                LastError = e;
                Trace.Write(e);
                return false;
            }
            finally
            {
                OnComplited?.Invoke(this, EventArgs.Empty);
            }
        }

        public async Task<bool> Compress()
        {
            LastError = null;

            try
            {
                await CompressInner(true);
                return true;
            }
            catch (Exception e)
            {
                LastError = e;
                Trace.Write(e);
                return false;
            }
            finally
            {
                OnComplited?.Invoke(this, EventArgs.Empty);
            }
        }

        #endregion

        #region Private methods

        private async Task ExtractInner(bool needToDeleteArch, bool needToClearFolder)
        {
            // очищаем папку
            if (Directory.Exists(_folderName) && needToClearFolder)
                Directory.Delete(_folderName, true);

            await Task.Run(() => ZipFile.ExtractToDirectory(_zipName, _folderName));

            if (needToDeleteArch && File.Exists(_zipName))
                File.Delete(_zipName);
        }

        private async Task CompressInner(bool rewriteFile)
        {
            if (File.Exists(_zipName))
            {
                if (!rewriteFile)
                    throw new Exception("File is existing");

                File.Delete(_zipName);
            }

            await Task.Run(() => ZipFile.CreateFromDirectory(_zipName, _folderName));
        }

        #endregion
    }
}
