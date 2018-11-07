namespace Core.Settings
{
    public interface ISettingsWorker
    {
        void Read();
        void Save();
        void DownloadAsync();
        void BackToDefaults();
    }
}