using System.ComponentModel;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Core.Settings
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum ModuleTypes
    {
        //[EnumMember(Value = nameof(None))]
        [Description("Отсутствует")]
        None,

        [EnumMember(Value = nameof(Default))]
        [Description("Официальная Mojang")]
        Default,

        [EnumMember(Value = nameof(Ely))]
        [Description("Ely.by")]
        Ely,

        [EnumMember(Value = nameof(Custom))]
        [Description("Модуль проекта")]
        Custom,
    }
}
