using LauncherFinal.Models.Settings.Interfases;

namespace LauncherFinal.Models.Settings
{
    class AuthModuleSettings : IAuthModuleSettings
    {
        public string AuthUri { get; set; }
        public bool StrictUsage { get; set; }
    }
}
