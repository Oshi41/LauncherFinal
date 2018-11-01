using LauncherCore.Settings;
using Newtonsoft.Json;

namespace LauncherFinal.Models.Settings
{
    class PasswordSettings : IPasswordSettings
    {
        [JsonProperty(PropertyName = "1")]
        public string Salt { get; set; }

        [JsonProperty(PropertyName = "2")]
        public string Password { get; set; }

        [JsonProperty(PropertyName = "3")]
        public string Encrypted { get; set; }
    }
}
