using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using LauncherCore.Settings;
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
                AuthModuleSettings = new AuthModuleSettings(),
                Servers = new List<IServer>
                {
                    new Server()
                }
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
            if (JsonConvert.DeserializeObject<ProjectConfig>(projectJson) is ProjectConfig conf)
                _settings.ProjectConfig = conf;


            var updateJson = await DownloadAndRead(_settings.UpdateConfigUrl);
            if (JsonConvert.DeserializeObject<UpdateConfig>(updateJson) is UpdateConfig upd)
                _settings.UpdateConfig = upd;
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
