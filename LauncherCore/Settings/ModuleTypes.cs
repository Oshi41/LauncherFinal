using System.ComponentModel;

namespace LauncherCore.Settings
{
    public enum ModuleTypes
    {
        [Description("Отсутствует")]
        None,

        [Description("Официальная Mojang")]
        Default,

        [Description("Ely.by")]
        Ely,

        [Description("Модуль проекта")]
        Custom,
    }
}
