using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace Core.Models
{
    /// <summary>
    /// Класс для работы со скачиванием файлов
    /// </summary>
    public class DirectWebClient : WebClient
    {
        public DirectWebClient()
        {
            IWebProxy wp = WebRequest.DefaultWebProxy;
            wp.Credentials = CredentialCache.DefaultCredentials; 
            Proxy = wp;;
        }

        /// <summary>
        /// Скачивает файл п оссылку в указанный путь
        /// </summary>
        /// <param name="uri">Ссылка</param>
        /// <param name="filePath">Куда скачиваем</param>
        /// <returns>Учитывает яндекс файлы, используя dokpub сервис</returns>
        public async Task DownloadFileAsync(string uri, string filePath = null)
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                filePath = Path.GetTempFileName();
            }

            if (!Uri.TryCreate(uri, UriKind.Absolute, out var link))
                return;

            var modified = ModifyLink(link);

            await DownloadFileTaskAsync(modified, filePath);
        }

        /// <summary>
        /// Модифицирует ссылку для скачивания, если требуется
        /// </summary>
        /// <param name="link"></param>
        /// <returns></returns>
        private Uri ModifyLink(Uri link)
        {
            if (link == null || !link.IsAbsoluteUri)
                return null;

            if (link.Host.Contains("yandex"))
            {
                return new Uri("https://getfile.dokpub.com/yandex/get/" + link);
            }

            return link;
        }

    }
}
