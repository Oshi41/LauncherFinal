namespace LauncherFinal.Models.Settings.Interfases
{
    public interface IAuthModuleSettings
    {
        /// <summary>
        /// Путь для регистраиции
        /// </summary>
        string AuthUri { get; set; }

        /// <summary>
        /// Использовать только этот тип аутентификации
        /// </summary>
        bool StrictUsage { get; set; }
    }
}
