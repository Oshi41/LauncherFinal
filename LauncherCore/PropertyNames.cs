using System;
using System.IO;

namespace LauncherCore
{
    public static class PropertyNames
    {
        /// <summary>
        /// Дефолтный путь к корневой папке
        /// </summary>
        public static string BaseLauncherPath { get; }

        static PropertyNames()
        {
            BaseLauncherPath = Path.Combine(Environment.GetFolderPath(
                    Environment.SpecialFolder.LocalApplicationData),
                "Universal Launcher");
        }

        /// <summary>
        /// Путь к папке настроек
        /// </summary>
        /// <param name="basePath">Путь к корневой папке лаунчера</param>
        /// <returns></returns>
        public static string GetConfigPath(string basePath)
        {
            try
            {
                return Path.Combine(basePath, "config", "settings.txt");
            }
            catch 
            {
                return string.Empty;
            }
        }
    }
}
