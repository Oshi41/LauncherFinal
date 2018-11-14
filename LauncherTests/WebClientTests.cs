using System;
using System.IO;
using System.Threading.Tasks;
using Core.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LauncherTests
{
    [TestClass]
    public class WebClientTests
    {
        private async Task DownloadInner(string s, string ext = "tmp")
        {
            string path = string.Empty;
            
            try
            {
                var client = new DirectWebClient();
                path = Path.ChangeExtension(Path.GetTempFileName(), ext);

                await client.DownloadFileAsync(s, path);

                Assert.IsTrue(File.Exists(path));
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }
            finally
            {
                if (File.Exists(path))
                    File.Delete(path);
            }

        }

        [DataTestMethod]
        [DataRow("https://upload.wikimedia.org/wikipedia/commons/thumb/8/80/Wikipedia-logo-v2.svg/103px-Wikipedia-logo-v2.svg.png")]
        [DataRow("https://upload.wikimedia.org/wikipedia/commons/thumb/1/1d/Www.wikipedia.org_screenshot_2018.png/800px-Www.wikipedia.org_screenshot_2018.png")]
        public void DownloadFile(string s)
        {
            Task.WaitAll(DownloadInner(s, "png"));
        }

        [DataTestMethod]
        [DataRow("https://yadi.sk/d/JoL9CHFZ3NGugD")]
        public void DownloadFileFromYandex(string s)
        {
            Task.WaitAll(DownloadInner(s));
        }

        [DataTestMethod]
        [DataRow("https://getfile.dokpub.com/yandex/get/https://yadi.sk/d/JoL9CHFZ3NGugD")]
        public void DownloadFileFromYandexDirect(string s)
        {
            Task.WaitAll(DownloadInner(s));
        }
    }
}
