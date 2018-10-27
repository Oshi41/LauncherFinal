using System.Collections.Generic;

namespace LauncherFinal.Models.Settings.Interfases
{
    public interface IProjectConfig
    {
        string ProjectSite { get; set; }

        //[JsonConverter(typeof(ConcreteTypeConverter<List<Server>>))]
        IList<IServer> Servers { get; set; }

        //[JsonConverter(typeof(ConcreteTypeConverter<AuthModuleSettings>))]
        IAuthModuleSettings AuthModuleSettings { get; set; }
    }
}
