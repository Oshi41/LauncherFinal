using Core.Settings;

namespace LauncherFinal.Models.Settings
{
    class AuthModuleSettings : IAuthModuleSettings
    {
        public string AuthUri { get; set; }
        public bool StrictUsage { get; set; }
        public ModuleTypes Type { get; set; }
    }
}
