using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace LauncherFinal.Models.Settings.Interfases
{
    public interface IUpdateConfig
    {
        string ExeUrl { get; set; }

        [JsonConverter(typeof(VersionConverter))]
        Version Version { get; set; }
    }
}
