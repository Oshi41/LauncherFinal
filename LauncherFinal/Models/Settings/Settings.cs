using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using LauncherFinal.JsonConverters;
using LauncherFinal.Models.Settings.Interfases;
using Newtonsoft.Json;

namespace LauncherFinal.Models.Settings
{
    public class Settings : ISettings
    {
        private string _javaPath;
        private int _megobytes;
        private IUpdateConfig _updateConfig;
        private IProjectConfig _projectConfig;
        private bool _savePass;
        private string _password;
        private string _login;
        private string _clientFolder;
        private string _configUrl;
        private bool _optimizeJava;
        private string _updateConfigUrl;

        public string JavaPath
        {
            get => _javaPath;
            set => SetProperty(ref _javaPath, value);
        }

        public int Megobytes
        {
            get => _megobytes;
            set => SetProperty(ref _megobytes, value);
        }

        public bool OptimizeJava
        {
            get => _optimizeJava;
            set => SetProperty(ref _optimizeJava, value);
        }

        public string ProjectConfigUrl
        {
            get => _configUrl;
            set => SetProperty(ref _configUrl, value);
        }

        public string UpdateConfigUrl
        {
            get => _updateConfigUrl;
            set => SetProperty(ref _updateConfigUrl, value);
        }

        public string ClientFolder
        {
            get => _clientFolder;
            set => SetProperty(ref _clientFolder, value);
        }

        public string Login
        {
            get => _login;
            set => SetProperty(ref _login, value);
        }

        public string Password
        {
            get => _password;
            set => SetProperty(ref _password, value);
        }

        public bool SavePass
        {
            get => _savePass;
            set => SetProperty(ref _savePass, value);
        }

        [JsonConverter(typeof(ConcreteTypeConverter<ProjectConfig>))]
        public IProjectConfig ProjectConfig
        {
            get => _projectConfig;
            set => SetProperty(ref _projectConfig, value);
        }

        [JsonConverter(typeof(ConcreteTypeConverter<UpdateConfig>))]
        public IUpdateConfig UpdateConfig
        {
            get => _updateConfig;
            set => SetProperty(ref _updateConfig, value);
        }

        public event EventHandler OnSettingsChanged;

        public Settings(string clientFolder)
        {
            _clientFolder = clientFolder;
        }

        #region Methods

        private void SetProperty<T>(ref T source, T value)
        {
            if (Equals(source, value))
                return;

            source = value;
            OnSettingsChanged?.Invoke(value, EventArgs.Empty);
        }

        public static Settings CreateDefault()
        {
            var basePath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "Universal Launcher");

            var settings = new Settings(basePath)
            {
                Megobytes = 1024 + 512
            };

            settings.JavaPath = settings.Find64Java();

            if (string.IsNullOrWhiteSpace(settings.JavaPath))
                settings.JavaPath = settings.FindAnyJava();

            return settings;
        }

        private string Find64Java()
        {
            try
            {
                var base64 = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles),
                    "Java").Replace(" (x86)", "");

                if (!Directory.Exists(base64))
                    return string.Empty;

                var allExe = Directory.GetFiles(base64, "javaw.exe", SearchOption.AllDirectories);
                if (allExe.Length == 0)
                    return string.Empty;

                var allJavaw = allExe.Where(x => Regex.IsMatch(x, "jre.*bin")).ToList();
                if (!allJavaw.Any())
                    return string.Empty;

                allJavaw.Sort();

                return allJavaw.LastOrDefault();

            }
            catch (Exception e)
            {
                Trace.Write(e);
                return string.Empty;
            }
        }

        private string FindAnyJava()
        {
            try
            {
                var javaFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles),
                    "Java");

                var files = Directory.GetFiles(javaFolder, "java.exe", SearchOption.AllDirectories).ToList();

                files.Sort();

                return files.LastOrDefault();
            }
            catch (Exception e)
            {
                Trace.Write(e);
                return string.Empty;
            }
        }

        #endregion
    }
}
