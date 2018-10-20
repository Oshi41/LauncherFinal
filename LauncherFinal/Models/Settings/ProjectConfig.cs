using System.Collections.Generic;
using LauncherFinal.JsonSerializer;
using LauncherFinal.Models.Settings.Interfases;
using Newtonsoft.Json;

namespace LauncherFinal.Models.Settings
{
    public class ProjectConfig : IProjectConfig
    {
        public string ProjectSite { get; set; }

        [JsonConverter(typeof(ConcreteTypeConverter<List<Server>>))]
        public IList<IServer> Servers { get; set; }
    }
}
