using System.ComponentModel;

namespace LauncherFinal.Models.AuthModules
{
    public enum ModuleTypes
    {
        [Description("Отсутствует")]
        None,

        [Description("Официальная Mojang")]
        Default,

        [Description("Ely.by")]
        Ely,
    }

    public class AuthModuleFactory
    {
        public IAuthModule GetByType(ModuleTypes type)
        {
            switch (type)
            {
                case ModuleTypes.Ely:
                    return new ElyAuthModule();

                case ModuleTypes.Default:
                    return new YggdrasilAuthModule("https://authserver.mojang.com/authenticate");

                default:
                    return new EmptyModule();
            }
        }
    }
}
