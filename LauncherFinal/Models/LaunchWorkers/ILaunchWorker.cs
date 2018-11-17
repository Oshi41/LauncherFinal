using System;

namespace LauncherFinal.Models.LaunchWorkers
{
    public interface ILaunchWorker
    {
        /// <summary>
        /// Возвращает текст для запуска из .bat файлов
        /// </summary>
        /// <returns></returns>
        string GetCmdArgs();

        /// <summary>
        /// Обычный запуск игры
        /// </summary>
        /// <param name="onExit">Вызывается после заверения процесса
        /// <para>Аргумент - есть ли ошибка</para></param>
        void RegularLaunch(EventHandler<bool> onExit);
    }
}