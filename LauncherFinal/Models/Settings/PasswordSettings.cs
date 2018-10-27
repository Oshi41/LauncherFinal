using LauncherFinal.Models.Settings.Interfases;

namespace LauncherFinal.Models.Settings
{
    class PasswordSettings : IPasswordSettings
    {
        public string Salt { get; set; }
        public string Password { get; set; }
        public string Encrypted { get; set; }
    }
}
