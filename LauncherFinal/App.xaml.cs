using System.Windows;
using LauncherFinal.Models.Settings.Interfases;

namespace LauncherFinal
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            var settingsWorker = IoCContainer.Instance.Resolve<ISettingsWorker>();

            settingsWorker.Read();
            settingsWorker.DownloadAsync();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            var settingsWorker = IoCContainer.Instance.Resolve<ISettingsWorker>();
            settingsWorker.Save();

            base.OnExit(e);
        }
    }
}
