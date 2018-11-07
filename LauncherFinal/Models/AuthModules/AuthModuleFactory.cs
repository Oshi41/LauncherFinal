using Core.Settings;

namespace LauncherFinal.Models.AuthModules
{
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

                case ModuleTypes.Custom:
                    var settings = IoCContainer.Instance.Resolve<ISettings>();
                    var uri = settings?.ProjectConfig?.AuthModuleSettings?.AuthUri;
                    return new YggdrasilAuthModule(uri);

                default:
                    return new EmptyModule();
            }
        }
    }
}
