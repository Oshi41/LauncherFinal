using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using ConsoleProgress;
using LauncherCore;
using LauncherCore.Settings;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Installer
{
    class Program
    {
        private static readonly string LauncherLink;
        private static readonly string ConfigLink;

        static void Main(string[] args)
        {
            if (args?.Any() == true && args[0] == "debug")
            {
                Debug();
            }
            else
            {
                DownloadAndInstall();
            }
        }

        static void Debug()
        {
            async Task Download(string text)
            {
                Console.WriteLine(text + "\n");

                var options = new ProgressBarOptions
                {
                    FilledCharColour = ConsoleColor.Green,
                    FilledChar = '█',
                    MaximumValue = 100,
                    NumberOfSquares = 50,
                    ShowBorders = false,
                    ShowPercentage = true,
                    UnfilledChar = ' ',
                    UnfilledCharColour = ConsoleColor.Red,
                };

                var bar = new ProgressBar(options);

                for (int i = 0; i < 100; i++)
                {
                    await Task.Delay(50);
                    bar.Report(i);
                }

                Console.WriteLine("\n\n");
            }

            Download("Скачиваю что-то одно").Wait();
            Download("Скачиваю что-то другое").Wait();
        }

        static void DownloadAndInstall()
        {
            var folder = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            var name = Path.Combine(folder, "Universal Launcher.exe");

            var downloadExe = DownloadFile(LauncherLink, name, "Скачиваем лаунчер").Result;
            if (downloadExe != null)
            {
                Console.WriteLine(downloadExe.Message);
                return;
            }

            var projectPath = Path.GetTempPath();
            var projectConfig = DownloadFile(ConfigLink, projectPath, "Загружаем конфигурацию").Result;
            if (projectConfig != null)
            {
                Console.WriteLine(projectConfig.Message);
                return;
            }

            var isWritten = WriteToSettings(projectPath);
            if (isWritten != null)
            {
                Console.WriteLine(isWritten.Message);
                return;
            }
        }

        static async Task<Exception> DownloadFile(string link, string filePath, string beginText)
        {
            try
            {
                Console.WriteLine(beginText);
                Console.WriteLine();

                var options = new ProgressBarOptions
                {
                    FilledCharColour = ConsoleColor.Green,
                    FilledChar = '█',
                    MaximumValue = 100,
                    NumberOfSquares = 50,
                    ShowBorders = false,
                    ShowPercentage = true,
                    UnfilledChar = ' ',
                    UnfilledCharColour = ConsoleColor.Red,
                };

                var bar = new ProgressBar(options);

                var client = new WebClient();
                client.DownloadProgressChanged += (sender, args) => { bar.Report(args.ProgressPercentage); };
                client.DownloadFileCompleted += (sender, args) => { bar.Report(100); };

                await client.DownloadFileTaskAsync(link, filePath);
                return null;
            }
            catch (Exception e)
            {
                return e;
            }
            finally
            {
                Console.WriteLine();
            }
        }

        static Exception WriteToSettings(string filePath)
        {
            try
            {
                var config = JObject.Parse(File.ReadAllText(filePath));
                var total = new JObject
                {
                    { nameof(IProjectConfig), config}
                };

                var outJson = JsonConvert.SerializeObject(total);
                File.WriteAllText(PropertyNames.GetConfigPath(PropertyNames.BaseLauncherPath), outJson);

                File.Delete(filePath);
                return null;
            }
            catch (Exception e)
            {
                return e;
            }
        }
    }
}
