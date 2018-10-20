using LauncherFinal.JsonSerializer;
using Newtonsoft.Json;

namespace LauncherFinal.Models.Settings.Interfases
{
    public interface ISettings
    {
        string JavaPath { get; set; }
        int Megobytes { get; set; }
        bool OptimizeJava { get; set; }

        string ConfigUrl { get; set; }
        string ClientFolder { get; set; }

        string Login { get; set; }
        string Password { get; set; }
        bool SavePass { get; set; }

        [JsonConverter(typeof(ConcreteTypeConverter<ProjectConfig>))]
        IProjectConfig ProjectConfig { get; set; }

        [JsonConverter(typeof(UpdateConfig))]
        IUpdateConfig UpdateConfig { get; set; }

    }
}
