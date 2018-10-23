using System.Collections.Generic;
using LauncherFinal.JsonConverters;
using Newtonsoft.Json;

namespace LauncherFinal.Models.Settings.Interfases
{
    public interface IProjectConfig
    {
        string ProjectSite { get; set; }

        [JsonConverter(typeof(ConcreteTypeConverter<List<Server>>))]
        IList<IServer> Servers { get; set; }
    }
}
