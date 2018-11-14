using System;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace Core.Models
{
    /// <summary>
    /// Класс для работы со скачиванием файлов
    /// </summary>
    public class DirectWebClient : IDisposable
    {
        private readonly WebClient _client;

        public DirectWebClient()
        {
            _client = new WebClient
            {
                Proxy = WebRequest.DefaultWebProxy,
            };

            _client.Proxy.Credentials = CredentialCache.DefaultCredentials;
        }

        public event DownloadProgressChangedEventHandler DownloadProgressChanged
        {
            add => _client.DownloadProgressChanged += value;
            remove => _client.DownloadProgressChanged -= value;
        }

        public event AsyncCompletedEventHandler DownloadFileCompleted
        {
            add => _client.DownloadFileCompleted += value;
            remove => _client.DownloadFileCompleted -= value;
        }

        public bool IsBusy => _client.IsBusy;

        /// <summary>
        /// Скачивает файл п оссылку в указанный путь
        /// </summary>
        /// <param name="uri">Ссылка</param>
        /// <param name="filePath">Куда скачиваем.
        /// <para>Если <see langword="null"/>, то берем случайный временный файл</para>
        /// </param>
        /// <returns>Учитывает яндекс файлы, используя dokpub сервис</returns>
        /// <exception cref="IOException"/>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="WebException"/>
        /// <exception cref="InvalidOperationException"/>
        public async Task DownloadFileAsync(string uri, string filePath = null)
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                filePath = Path.GetTempFileName();
            }

            if (!Uri.TryCreate(uri, UriKind.Absolute, out var link))
                return;

            var modified = ModifyLink(link);

            await _client.DownloadFileTaskAsync(modified, filePath);
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

            if (link.Host.Contains("yadi"))
            {
                return new Uri("https://getfile.dokpub.com/yandex/get/" + link);
            }

            return link;
        }

        public void CancelAsync()
        {
            _client?.CancelAsync();
        }

        public void Dispose()
        {
            _client?.Dispose();
        }
    }
}
