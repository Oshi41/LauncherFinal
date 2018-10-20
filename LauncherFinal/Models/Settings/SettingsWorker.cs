using System;
using System.Diagnostics;
using System.IO;
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
            var jsonSettings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
            };

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

        public async void DownloadAsync()
        {
            DownloadManager manager = null;
            try
            {
                manager = new DownloadManager(_settings.ConfigUrl, interval: -1);

                var path = await manager.Download();
                if (manager.IsError)
                    throw manager.LastError;

                var json = File.ReadAllText(path);
                _settings.ProjectConfig = JsonConvert.DeserializeObject<ProjectConfig>(json);
            }
            catch (Exception e)
            {
                Trace.Write(e);
            }
            finally
            {
                manager?.Dispose();
            }
        }
    }
}
