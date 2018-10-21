namespace LauncherFinal.Models.AuthModules
{
    public enum ModuleTypes
    {
        None,
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

                default:
                    return new EmptyModule();
            }
        }
    }
}
