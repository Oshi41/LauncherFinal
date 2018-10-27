using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using LauncherFinal.Models.Settings.Interfases;
using Newtonsoft.Json;

namespace LauncherFinal.Models.Settings
{
    public class SettingsWorker : ISettingsWorker
    {
        private readonly string _settingsPath;
        private readonly ISettings _settings;

        public SettingsWorker(string filePath, ISettings settings)
        {
            _settingsPath = filePath;
            _settings = settings;

            try
            {
                var directory = Path.GetDirectoryName(_settingsPath);

                if (!Path.IsPathRooted(directory))
                    throw new Exception("Wrong folder");

                if (!Directory.Exists(directory))
                    Directory.CreateDirectory(directory);
            }
            catch (Exception e)
            {
                Trace.Write(e);
                throw;
            }
        }

        public void Save()
        {
            var jsonSettings = new JsonSerializerSettings();

#if DEBUG
            _settings.ProjectConfig = new ProjectConfig
            {
                AuthModuleSettings = new AuthModuleSettings()
            };
            _settings.UpdateConfig = new UpdateConfig();
#else
            jsonSettings.NullValueHandling = NullValueHandling.Ignore;
#endif

            var json = JsonConvert.SerializeObject(_settings, Formatting.Indented, jsonSettings);

            try
            {
                File.WriteAllText(_settingsPath, json);
            }
            catch (Exception e)
            {
                Trace.Write(e);
            }
        }

        public void Read()
        {
            try
            {
                var json = File.ReadAllText(_settingsPath);
                JsonConvert.PopulateObject(json, _settings);
            }
            catch (Exception e)
            {
                Trace.Write(e);
            }
        }

        public void BackToDefaults()
        {
            var json = JsonConvert.SerializeObject(Settings.CreateDefault(), Formatting.Indented);
            JsonConvert.PopulateObject(json, _settings);
            DownloadAsync();
        }

        public async void DownloadAsync()
        {
            var projectJson = await DownloadAndRead(_settings.ProjectConfigUrl);
            _settings.ProjectConfig = JsonConvert.DeserializeObject<ProjectConfig>(projectJson);

            var updateJson = await DownloadAndRead(_settings.UpdateConfigUrl);
            _settings.UpdateConfig = JsonConvert.DeserializeObject<UpdateConfig>(updateJson);
        }

        private async Task<string> DownloadAndRead(string url)
        {
            DownloadManager manager = null;

            try
            {
                manager = new DownloadManager(url, interval: -1);

                var path = await manager.Download();
                if (manager.IsError)
                    throw manager.LastError;

                return File.ReadAllText(path);
            }
            catch (Exception e)
            {
                Trace.Write(e);
                return string.Empty;
            }
            finally
            {
                manager?.Dispose();
            }
        }
    }
}
