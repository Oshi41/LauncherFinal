﻿using System;
using LauncherFinal.JsonSerializer;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace LauncherFinal.Models.Settings.Interfases
{
    public interface ISettings
    {
        string JavaPath { get; set; }
        int Megobytes { get; set; }
        bool OptimizeJava { get; set; }

        string ProjectConfigUrl { get; set; }
        string UpdateConfigUrl { get; set; }
        string ClientFolder { get; set; }

        string Login { get; set; }
        string Password { get; set; }
        bool SavePass { get; set; }

        [JsonConverter(typeof(ConcreteTypeConverter<ProjectConfig>))]
        IProjectConfig ProjectConfig { get; set; }

        [JsonConverter(typeof(ConcreteTypeConverter<UpdateConfig>))]
        IUpdateConfig UpdateConfig { get; set; }

        event EventHandler OnSettingsChanged;
    }
}
