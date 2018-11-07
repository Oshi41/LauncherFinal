using System.Collections.Generic;

namespace Core.Settings
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
