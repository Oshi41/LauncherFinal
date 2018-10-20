using System.Collections.Generic;
using LauncherFinal.JsonSerializer;
using Newtonsoft.Json;

namespace LauncherFinal.Models.Settings.Interfases
{
    public interface IProjectConfig
    {
        [JsonConverter(typeof(ConcreteTypeConverter<List<Server>>))]
        IList<IServer> Servers { get; set; }
    }
}
