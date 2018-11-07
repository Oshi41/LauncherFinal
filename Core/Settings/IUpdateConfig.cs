using System;

namespace Core.Settings
{
    public interface IUpdateConfig
    {
        string ExeUrl { get; set; }

        //[JsonConverter(typeof(VersionConverter))]
        Version Version { get; set; }
    }
}
