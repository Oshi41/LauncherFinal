using System.Collections.Generic;
using LauncherFinal.Models.Settings.Interfases;

namespace LauncherFinal.Models.Settings
{
    public class ProjectConfig : IProjectConfig
    {
        public IList<IServer> Servers { get; set; }
    }
}
