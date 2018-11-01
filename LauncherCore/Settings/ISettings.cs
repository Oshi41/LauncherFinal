using System;

namespace LauncherCore.Settings
{
    public interface ISettings
    {
        string JavaPath { get; set; }
        int Megabytes { get; set; }
        bool OptimizeJava { get; set; }

        string ProjectConfigUrl { get; set; }
        string UpdateConfigUrl { get; set; }
        string ClientFolder { get; set; }

        string Login { get; set; }
        IPasswordSettings Password { get; set; }
        bool SavePass { get; set; }

        //[JsonConverter(typeof(ConcreteTypeConverter<ProjectConfig>))]
        IProjectConfig ProjectConfig { get; set; }

        //[JsonConverter(typeof(ConcreteTypeConverter<UpdateConfig>))]
        IUpdateConfig UpdateConfig { get; set; }

        event EventHandler OnSettingsChanged;
    }
}
