namespace LauncherFinal.Models.Settings.Interfases
{
    public interface ISettingsWorker
    {
        void Read();
        void Save();

        void DownloadAsync();
    }
}