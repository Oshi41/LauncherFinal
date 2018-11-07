using System;
using Core.Settings;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace LauncherFinal.Models.Settings
{
    class UpdateConfig : IUpdateConfig
    {
        public string ExeUrl { get; set; }

        [JsonConverter(typeof(VersionConverter))]
        public Version Version { get; set; }
    }
}
