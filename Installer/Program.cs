﻿using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using ConsoleProgress;
using Core;
using Core.Json;
using Core.Models;
using Newtonsoft.Json.Linq;

namespace Installer
{
    class Program
    {
        private static readonly string LauncherLink;
        private static readonly string ProjectLink;
        private static readonly string UpdateLink;

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

            var project = DownloadAndParse(ProjectLink, Path.GetTempPath(), "Загружаем конфигурацию").Result;
            var update = DownloadAndParse(UpdateLink, Path.GetTempPath(), "Проверяем обновления").Result;

            var serializer = new SettingsSerializer();
            var settings = serializer.WriteSettings(update, UpdateLink, project, ProjectLink);

            File.WriteAllText(PropertyNames.GetConfigPath(PropertyNames.BaseLauncherPath), settings.ToStringIgnoreNull());
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

                var client = new DirectWebClient();
                client.DownloadProgressChanged += (sender, args) => { bar.Report(args.ProgressPercentage); };
                client.DownloadFileCompleted += (sender, args) => { bar.Report(100); };

                await client.DownloadFileAsync(link, filePath);
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

        static async Task<JObject> DownloadAndParse(string link, string filePath, string beginText)
        {
            var e = await DownloadFile(link, filePath, beginText);
            if (e != null)
            {
                Console.WriteLine(e.Message);
                return null;
            }

            try
            {
                return JObject.Parse(File.ReadAllText(filePath));
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
                return null;
            }
            finally
            {
                if (File.Exists(filePath))
                    File.Delete(filePath);
            }
        }
    }
}

