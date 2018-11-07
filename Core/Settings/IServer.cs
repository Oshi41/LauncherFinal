using System.Collections.Generic;

namespace Core.Settings
{
    public interface IServer
    {
        /// <summary>
        /// Адрес сервера
        /// </summary>
        string Address { get; }

        /// <summary>
        /// Имя сервера
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Ссылка на скачивание клиентских файлов
        /// </summary>
        string DownloadLink { get; }

        /// <summary>
        /// Хэш-суммы папки сервера
        /// <para>Путь к папке -> Хэш</para>
        /// </summary>
        //[JsonConverter(typeof(ConcreteTypeConverter<Dictionary<string, string>>))]
        IDictionary<string, string> DirHashCheck { get; }
    }
}
