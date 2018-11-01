using System.Collections.Generic;
using LauncherCore.Settings;
using LauncherFinal.JsonConverters;
using Newtonsoft.Json;

namespace LauncherFinal.Models.Settings
{
    public class ProjectConfig : IProjectConfig
    {
        public string ProjectSite { get; set; }

        [JsonConverter(typeof(ConcreteTypeConverter<List<Server>>))]
        public IList<IServer> Servers { get; set; }

        [JsonConverter(typeof(ConcreteTypeConverter<AuthModuleSettings>))]
        public IAuthModuleSettings AuthModuleSettings { get; set; }
    }
}
