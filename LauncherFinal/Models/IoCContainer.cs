using System;
using System.IO;
using LauncherFinal.Models.Settings;
using LauncherFinal.Models.Settings.Interfases;
using Unity;

// ReSharper disable once CheckNamespace
namespace LauncherFinal
{
    public class IoCContainer
    {
        #region Singleton implementation

        private static IoCContainer _instance;
        public static IoCContainer Instance => _instance ?? (_instance = new IoCContainer());

        #endregion

        #region Private

        private readonly UnityContainer _container = new UnityContainer();

        #endregion

        private IoCContainer()
        {
            var basePath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "Universal Launcher");
            var settingsFilePath = Path.Combine(basePath, "settings.txt");


            var settings = new Settings {ClientFolder = basePath};
            var worker = new SettingsWorker(settingsFilePath, settings);
            worker.Read();

            RegisterAsSigleton<ISettings>(settings);
            RegisterAsSigleton<ISettingsWorker>(worker);
        }

        #region Methods

        public T Resolve<T>()
        {
            return _container.Resolve<T>();
        }

        /// <summary>
        /// Регистрируем объект
        /// </summary>
        /// <typeparam name="T">Тип регистрируемого объекта</typeparam>
        /// <param name="value"></param>
        private void RegisterAsSigleton<T>(object value)
        {
            if (value == null)
                return;

            _container.RegisterInstance(typeof(T), value);
        }

        #endregion
    }
}
