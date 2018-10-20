using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using LauncherFinal.Models.Settings.Interfases;
using Newtonsoft.Json;

namespace LauncherFinal.Models.Settings
{                         
    public class Settings : ISettings
    {
        public string JavaPath { get; set; }
        public int Megobytes { get; set; }
        public bool OptimizeJava { get; set; }

        public string ConfigUrl { get; set; }
        public string ClientFolder { get; set; }

        public string Name { get; set; }
        public string Password { get; set; }
        public bool SavePass { get; set; }

        [JsonConverter(typeof(ProjectConfig))]
        public IProjectConfig ProjectConfig { get; set; }

        public Settings(string clientFolder)
        {
            ClientFolder = clientFolder;

            Megobytes = 1024 + 512;

            JavaPath = Find64Java();
            if (string.IsNullOrWhiteSpace(JavaPath))
                JavaPath = FindAnyJava();
        }

        #region Methods

        private string Find64Java()
        {
            try
            {
                var base64 = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles),
                    "Java").Replace(" (x86)", "");

                if (!Directory.Exists(base64))
                    return string.Empty;

                var allExe = Directory.GetFiles(base64, "javaw.exe", SearchOption.AllDirectories);
                if (allExe.Length == 0)
                    return string.Empty;

                var allJavaw = allExe.Where(x => Regex.IsMatch(x, "jre.*bin")).ToList();
                if (!allJavaw.Any())
                    return string.Empty;

                allJavaw.Sort();

                return allJavaw.LastOrDefault();

            }
            catch (Exception e)
            {
                Trace.Write(e);
                return string.Empty;
            }
        }

        private string FindAnyJava()
        {
            try
            {
                var javaFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles),
                    "Java");

                var files = Directory.GetFiles(javaFolder, "java.exe", SearchOption.AllDirectories).ToList();

                files.Sort();

                return files.LastOrDefault();
            }
            catch (Exception e)
            {
                Trace.Write(e);
                return string.Empty;
            }
        }

        #endregion
    }
}
