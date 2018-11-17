using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using LauncherFinal.Helper;
using Newtonsoft.Json.Linq;

namespace LauncherFinal.Models.LaunchWorkers
{
    public class ForgeLaunchWorkerNew : ILaunchWorker
    {
        #region Static Fields

        /// <summary>
        /// список загружаемых библиотек
        /// </summary>
        private static readonly List<string> LibraryNameRegexes = new List<string>
        {
            "forge",
            "launchwrapper",
            "asm-all",
            "jline",
            "akka-actor_",
            "config",
            "scala-actors-migration_",
            "scala-compiler",
            "scala-continuations-library_",
            "scala-continuations-plugin_",
            "scala-library",
            "scala-parser-combinators_",
            "scala-reflect",
            "scala-swing_",
            "scala-xml",
            "lzma",
            "jopt-simple",
            "vecmath",
            "trove4j",
            "MercuriusUpdater",
            "netty",
            "oshi-core",
            "jna",
            "platform",
            "icu4j-core-mojang",
            "codecjorbis",
            "codecwav",
            "libraryjavasound",
            "librarylwjglopenal",
            "soundsystem",
            @"netty-all.*.Final",
            "guava",
            "commons-lang3",
            "commons-io",
            "commons-codec",
            "jinput",
            "jutils",
            "gson",
            "mojang.*authlib",
            "realms",
            "commons-compress",
            "httpclient",
            "commons-logging",
            "httpcore",
            @"fastutil.*_mojang",
            "log4j-api",
            "log4j-core",
            @"lwjgl.*nightly",
            @"lwjgl_util.*nightly",
        };

        /// <summary>
        /// Доп аргументы Java.
        /// <para>String.Format - передать мин. и макс. память!</para>
        /// </summary>
        private static readonly string _javaExpArgs =
            " -XX:+DisableExplicitGC -XX:+UseParNewGC -XX:+ScavengeBeforeFullGC " +
            "-XX:+CMSScavengeBeforeRemark -XX:+UseNUMA -XX:+CMSParallelRemarkEnabled " +
            "-XX:MaxTenuringThreshold=15 -XX:MaxGCPauseMillis=30 -XX:GCPauseIntervalMillis=150 " +
            "-XX:+UseAdaptiveGCBoundary -XX:-UseGCOverheadLimit -XX:+UseBiasedLocking -XX:SurvivorRatio=8 " +
            "-XX:TargetSurvivorRatio=90 -XX:MaxTenuringThreshold=15 " +
            "-XX:+UseFastAccessorMethods -XX:+UseCompressedOops -XX:+OptimizeStringConcat -XX:+AggressiveOpts " +
            "-XX:+UseCodeCacheFlushing -XX:ParallelGCThreads=5 " +
            "-XX:ReservedCodeCacheSize={0}m -XX:SoftRefLRUPolicyMSPerMB={1} ";

        #endregion

        #region Fields

        private readonly string _minecraftPath;
        private readonly string _javaPath;
        private readonly bool _useExpArgs;
        private readonly string _login;
        private readonly string _accessToken;

        private Size _windowSize = new Size(925, 530);

        private int _maxMemory = 1024;
        private int _minMemory = 4096;

        private string _forgeNativesPath;
        private string _libPaths;
        private string _forgeJarPath;
        private Version _minecraftVersion;
        private string _minecraftAssetsPath;

        #endregion

        public ForgeLaunchWorkerNew(string minecraftPath,
            string javaPath,
            string login,
            string accessToken,
            bool useExpArgs)
        {
            _minecraftPath = minecraftPath;
            _javaPath = javaPath;
            _login = login;
            _accessToken = accessToken;
            _useExpArgs = useExpArgs;

            CheckFolderAndThrow(_minecraftPath);
            CheckPathsAndThrow(_javaPath);

            Initialize();
        }

        private void Initialize()
        {
            FillLibrariesPaths();
            FindForgeNatives();
            FindForgeVersion();
            FindMinecraftAssets();
        }

        #region Main Launch

        public void RegularLaunch(EventHandler<bool> onExit)
        {
            var startInfo = new ProcessStartInfo
            {
                FileName = _javaPath,
                CreateNoWindow = true,
                RedirectStandardError = true,
                RedirectStandardOutput = true,
                WorkingDirectory = _minecraftPath,
                WindowStyle = ProcessWindowStyle.Hidden,
                Arguments = GetLaunchArgs(),
                UseShellExecute = false
            };

            var process = new Process { StartInfo = startInfo };

            process.Exited += (sender, args) =>
            {
               onExit?.Invoke(sender, process.ExitCode != 0);
            };

            process.Start();
        }

        public string GetCmdArgs()
        {
            return $"\"{_javaPath}\" " + GetLaunchArgs();
        }

        #endregion

        #region Helping Methods

        private string GetLaunchArgs()
        {
            var result = $"-Xmn{_minMemory}M " +
                         $"-Xmx{_maxMemory}M " +
                         $"-Djava.library.path=\"{_forgeNativesPath}\" " +
                         $" -cp \"{_libPaths};{_forgeJarPath}\" " +
                         "net.minecraft.launchwrapper.Launch " +
                         $"--username {_login} " +
                         $"--version  Forge {_minecraftVersion.ToString(3)} " +
                         $"--gameDir \"{_minecraftPath}\" " +
                         $"--assetsDir \"{_minecraftAssetsPath}\" " +
                         "--tweakClass net.minecraftforge.fml.common.launcher.FMLTweaker " +
                         $"--width {_windowSize.Width} --height {_windowSize.Height} " +
                         $"--accessToken {_accessToken ?? "null"} " +
                         "--userProperties {} " +
                         $"--assetIndex {_minecraftVersion.ToString(2)} " +
                         "-Dfml.ignoreInvalidMinecraftCertificates=true -Dfml.ignorePatchDiscrepancies=true"
                ;

            if (_useExpArgs)
            {
                result += string.Format(_javaExpArgs, _minMemory, _maxMemory);
            }

            return result;
        }

        /// <summary>
        /// Проверяет наличие папок
        /// </summary>
        /// <param name="paths"></param>
        private void CheckFolderAndThrow(params string[] paths)
        {
            if (paths == null)
                return;

            foreach (var path in paths)
            {
                if (!Directory.Exists(path))
                    throw new Exception($"Can't find folder - {path}");
            }
        }

        /// <summary>
        /// Проверяет наличие файлов
        /// </summary>
        /// <param name="paths"></param>
        private void CheckPathsAndThrow(params string[] paths)
        {
            if (paths == null)
                return;

            foreach (var path in paths)
            {
                if (!File.Exists(path))
                    throw new Exception($"Can't find jarFile - {path}");
            }
        }

        /// <summary>
        /// Заполняет аргументы запуска необходимыми либами
        /// </summary>
        private void FillLibrariesPaths()
        {
            // нашли папку с либами
            var dirs = Directory.GetDirectories(_minecraftPath, "libraries", SearchOption.AllDirectories);
            var dir = PathsHelper.GetTopPath(dirs);

            // проерили
            CheckFolderAndThrow(dir);

            // вытащили все jar
            var allJars = Directory.GetFiles(dir, "*.jar", SearchOption.AllDirectories).ToList();

            var selected = new List<string>();

            // ищем те, которые нужны
            foreach (var libName in LibraryNameRegexes)
            {
                var libs = allJars.Where(x => Regex.IsMatch(x, libName)).ToList();

                if (libs.IsNullOrEmpty())
                    continue;

                var shortest = PathsHelper.GetShortestName(libs);
                selected.Add(shortest);
                allJars.Remove(shortest);
            }

            _libPaths = string.Join(";", selected);
        }

        /// <summary>
        /// Находит папку нативных библиотек Форджа
        /// </summary>
        private void FindForgeNatives()
        {
            var dirs = Directory.GetDirectories(_minecraftPath, "natives", SearchOption.AllDirectories);
            _forgeNativesPath = PathsHelper.GetTopPath(dirs);

            CheckFolderAndThrow(_forgeNativesPath);
        }

        /// <summary>
        /// Находит главный forge.jar и его версию
        /// </summary>
        private void FindForgeVersion()
        {
            var forges = Directory.GetFiles(_minecraftPath, "*forge*.jar", SearchOption.AllDirectories)
                // исключаем папку mods
                .Where(x => !PathsHelper.PlacedInFolder(x, "mods"))
                .ToList();

            _forgeJarPath = PathsHelper.GetTopPath(forges);

            CheckPathsAndThrow(_forgeJarPath);

            _minecraftVersion = ParseJarVersion(_forgeJarPath);
        }

        /// <summary>
        /// Ищет папку assets Майнкрафта
        /// </summary>
        private void FindMinecraftAssets()
        {
            var dirs = Directory.GetDirectories(_minecraftPath, "assets", SearchOption.AllDirectories);
            _minecraftAssetsPath = PathsHelper.GetTopPath(dirs);

            CheckFolderAndThrow(_minecraftAssetsPath);
        }

        #region Parse version

        /// <summary>
        /// 
        /// </summary>
        /// <param name="jarFile"></param>
        private Version ParseJarVersion(string jarFile)
        {
            try
            {
                //
                // Смотрю на файл json рядом с jar с таким же имененм
                //

                var path = Path.ChangeExtension(jarFile, "json");
                return ParseFromJson(File.ReadAllText(path));
            }
            catch
            {
                try
                {
                    //
                    // Распаковываю jar файл и смотрю там version
                    //

                    var zip = ZipFile.OpenRead(jarFile);
                    var allJsons = zip
                        .Entries.Where(x => x.Name.Contains("json"))
                        .ToList();

                    var top = PathsHelper.GetTopPath(allJsons.Select(x => x.FullName));
                    var version = allJsons.FirstOrDefault(x => string.Equals(x.FullName, top));
                    if (version != null)
                    {
                        using (var stream = version.Open())
                        {
                            using (var reader = new StreamReader(stream))
                            {
                                return ParseFromJson(reader.ReadToEnd());
                            }
                        }
                    }
                }
                catch 
                {
                    // ignored
                }
            }

            return new Version();
        }

        /// <summary>
        /// Смотрит на jar свойство, потом на id
        /// </summary>
        /// <param name="jsonContent"></param>
        /// <returns></returns>
        private Version ParseFromJson(string jsonContent)
        {
            try
            {
                var json = JObject.Parse(jsonContent);

                if (json.TryGetValue("jar", out var jar))
                {
                    return Version.Parse(jar.ToString());
                }

                if (json.TryGetValue("id", out var id))
                {
                    var text = id.ToString();
                    text = text.Substring(0, text.IndexOf("-", StringComparison.Ordinal));
                    return Version.Parse(text);
                }
            }
            catch
            {
                // ignored
            }

            return new Version();
        }

        #endregion

        #endregion
    }
}
