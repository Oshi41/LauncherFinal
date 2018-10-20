using LauncherFinal.Models.Settings.Interfases;
using Newtonsoft.Json;

namespace LauncherFinal.Models.Settings
{                         
    public class Settings : ISettings
    {
        public string JavaPath { get; set; }
        public int Megobytes { get; set; }
        public bool OptimizeJava { get; set; }

        public string ConfigUrl { get; set; }
        public string ClientFolder { get; set; }

        public string Name { get; set; }
        public string Password { get; set; }
        public bool SavePass { get; set; }

        [JsonConverter(typeof(ProjectConfig))]
        public IProjectConfig ProjectConfig { get; set; }
    }
}
