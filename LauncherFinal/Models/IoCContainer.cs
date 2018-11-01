﻿using System.IO;
using LauncherCore;
using LauncherCore.Settings;
using LauncherFinal.Models.Settings;
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
            var settings = Settings.CreateDefault();
            var worker = new SettingsWorker(PropertyNames.GetConfigPath(settings.ClientFolder),
                settings);

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
