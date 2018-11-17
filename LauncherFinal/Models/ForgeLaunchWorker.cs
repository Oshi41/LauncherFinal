using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using Newtonsoft.Json.Linq;

namespace LauncherFinal.Models
{
    public class ForgeLaunchWorker
    {
        #region Fields

        // список загружаемых библиотек
        private static List<string> _libraries = new List<string>
        {
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

        private readonly string _javaExpArgs =
            " -XX:+DisableExplicitGC -XX:+UseParNewGC -XX:+ScavengeBeforeFullGC " +
            "-XX:+CMSScavengeBeforeRemark -XX:+UseNUMA -XX:+CMSParallelRemarkEnabled " +
            "-XX:MaxTenuringThreshold=15 -XX:MaxGCPauseMillis=30 -XX:GCPauseIntervalMillis=150 " +
            "-XX:+UseAdaptiveGCBoundary -XX:-UseGCOverheadLimit -XX:+UseBiasedLocking -XX:SurvivorRatio=8 " +
            "-XX:TargetSurvivorRatio=90 -XX:MaxTenuringThreshold=15 " +
            "-XX:+UseFastAccessorMethods -XX:+UseCompressedOops -XX:+OptimizeStringConcat -XX:+AggressiveOpts " +
            "-XX:+UseCodeCacheFlushing -XX:ParallelGCThreads=5 " +
            "-XX:ReservedCodeCacheSize={0}m -XX:SoftRefLRUPolicyMSPerMB={1} ";

        private string _version;
        private readonly string _javaPath;
        private readonly string _accessToken;
        private string _assetsIndex;
        private string _libArgs;
        private int _minMemory = 1024;
        private int _maxMemory = 4096;
        private string _forgeLibPath;
        private string _versionForgePath;

        private readonly bool _useExpArgs;

        private string _natives;

        private readonly string _login;
        //private string uuid;

        private Size _size = new Size(925, 530);

        #endregion

        #region Properties

        public string MinecraftFolder { get; }

        #endregion

        #region Constructors

        //public ForgeLaunchWorker(string folder, 
        //    string accessToken,
        //    bool useExpArgs)
        //{
        //    _accessToken = accessToken;
        //    _useExpArgs = useExpArgs;
        //    MinecraftFolder = folder;

        //    if (!FindVersionsExperimantal())
        //        return;

        //    Init();
        //}

        public ForgeLaunchWorker(string folder,
            string accessToken,
            string javaPath,
            string login,
            bool useExpArgs)
        {
            _accessToken = accessToken;
            _login = login;
            _useExpArgs = useExpArgs;
            _javaPath = javaPath;

            if (!File.Exists(_javaPath))
                throw new Exception("Can't find java by this path - " + _javaPath);

            MinecraftFolder = folder;
            if (!Directory.Exists(MinecraftFolder))
                throw new Exception("Can't find Minecraft folder by this path - " + MinecraftFolder);

            Init();
        }

        private void Init()
        {
            FillLibsArguments();
            FindNativesPath();

            if (!FindForgePaths())
                throw new Exception("Can't find Forge library");
        }

        #endregion

        #region Public methods

        public void RegularLaunch(EventHandler onExit)
        {
            var startInfo = new ProcessStartInfo
            {
                FileName = _javaPath,
                CreateNoWindow = true,
                RedirectStandardError = true,
                RedirectStandardOutput = true,
                WorkingDirectory = MinecraftFolder,
                WindowStyle = ProcessWindowStyle.Hidden,
                Arguments = GetLauncArgs(),
                UseShellExecute = false
            };

            var process = new Process { StartInfo = startInfo };
            process.Exited += onExit;

            process.Start();
        }

        #endregion

        #region methods

        private string GetLauncArgs()
        {
            var result = $"-Xmn{_minMemory}M -Xmx{_maxMemory}M -Djava.library.path=\"{_natives}\"";
            result += $" -cp \"{_forgeLibPath};{_libArgs}{_versionForgePath}\"";
            result += $" net.minecraft.launchwrapper.Launch" +
                      $" --username {_login}";
            result += $" --version {_version}";
            result += $" --gameDir {MinecraftFolder}";
            result += $" --assetsDir {Path.Combine(MinecraftFolder, "assets")}";
            result += " --tweakClass net.minecraftforge.fml.common.launcher.FMLTweaker";
            result += $" --width {_size.Width} --height {_size.Height}";
            result += $" --accessToken {_accessToken ?? "null"}";
            result += " --userProperties {}";
            result += $" --assetIndex {_assetsIndex}";
            result += " -Dfml.ignoreInvalidMinecraftCertificates=true -Dfml.ignorePatchDiscrepancies=true";

            if (_useExpArgs)
            {
                result += _javaExpArgs;
            }

            return result;
        }

        public string GetCmdArgs()
        {
            return $"\"{_javaPath}\" " + GetLauncArgs();
        }

        #endregion

        #region Methods

        //private void ParseAssetIndex()
        //{
        //    
        //}

        /// <summary>
        /// Проводит поиск Forge
        /// </summary>
        /// <returns></returns>
        private bool FindForgePaths()
        {
            var files = Directory.GetFiles(MinecraftFolder, "*forge*.jar", SearchOption.AllDirectories)
                .OrderByDescending(x => x)
                .ToArray();
            if (files.Any())
            {
                _forgeLibPath = files.FirstOrDefault();
                return SetVersion(GetJarVersion(_forgeLibPath).Item2);
            }

            files = Directory.GetFiles(MinecraftFolder, "*.jar");
            var map = files.ToDictionary(x => x, GetJarVersion);

            var first = map
                .FirstOrDefault(x => x.Value.Item1.ToLower().Contains("forge"));

            if (!File.Exists(first.Key))
                return false;

            _forgeLibPath = first.Key;
            _versionForgePath = _forgeLibPath;
            return SetVersion(first.Value.Item2);
        }

        private bool SetVersion(Version version)
        {
            if (version.Major != 0)
            {
                _assetsIndex = $"{version.Major}.{version.Minor}";
                _version = $"Forge {_assetsIndex}.{version.Build}";
            }

            return version.Major != 0;
        }

        private void FindNativesPath()
        {
            try
            {
                var info = new DirectoryInfo(MinecraftFolder);
                var dirs = info.GetDirectories("*", SearchOption.AllDirectories);
                var first = dirs.FirstOrDefault(x => string.Equals(x.Name, "natives"));

                if (first != null)
                {
                    _natives = first.FullName;
                }
            }
            catch (Exception e)
            {
                // ignored
            }
        }

        /// <summary>
        /// Добавляем список либ
        /// </summary>
        private void FillLibsArguments()
        {
            var dir = new DirectoryInfo(Path.Combine(MinecraftFolder, "libraries"));
            if (!dir.Exists)
                return;

            var jars = Directory.GetFiles(dir.FullName, "*.jar", SearchOption.AllDirectories).ToList();
            var args = "";

            foreach (var lib in _libraries)
            {
                var all = jars.Where(x => Regex.IsMatch(x, lib)).ToList();

                // выбираем самое короткое имя
                if (all.Count() > 1)
                {
                    var shortest = all.Min(x => x.Length);
                    all = all.Where(x => x.Length == shortest).ToList();
                }

                // либо позднюю версию
                if (all.Count() > 1)
                {
                    var dict = all
                        .ToDictionary(x => x, GetVersion)
                        .OrderBy(x => x.Value)
                        .ToDictionary(x => x.Key, x => x.Value);

                    all = new List<string>
                    {
                        dict.LastOrDefault().Key
                    };
                }

                var find = all.FirstOrDefault();
                if (find == null)
                {
                    Trace.WriteLine("Can't find lib - " + lib);
                    continue;
                }

                args += find + ";";
                jars.Remove(find);
            }

            _libArgs = args.TrimEnd(';');
        }

        /// <summary>
        /// Получаем версию из имени файла (Experimantal)
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        private Version GetVersion(string text)
        {
            var corrected = text.Replace("-", ".").Replace("—", ".").Trim(' ', '.');
            var versionRaw = new string(corrected.Where(x => Regex.IsMatch(x.ToString(), "[0-9, \\.]")).ToArray());

            var cutted = string.Join(".", versionRaw.Split('.').Take(4));

            if (Version.TryParse(cutted, out var result))
                return result;

            return new Version(0, 0, 0, 0);
        }

        //private bool FindVersionsExperimantal()
        //{
        //    // путь к нативным либам forge
        //    var path = Path.Combine(MinecraftFolder, "libraries", "net", "minecraftforge", "forge");

        //    // по идее, располагается только одна либа.
        //    var dirs = Directory.GetDirectories($"{path}\\", "*", SearchOption.TopDirectoryOnly);
        //    var forgeDir = dirs.FirstOrDefault();
        //    if (string.IsNullOrEmpty(forgeDir))
        //        return false;

        //    var version = GetVersion(Path.GetFileName(forgeDir));

        //    if (version.Major != 0)
        //    {
        //    }

        //    return true;
        //}

        /// <summary>
        /// First - id из json описания jar
        /// <para> Second - jar поле из описания jar </para>
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private Tuple<string, Version> GetJarVersion(string path)
        {
            try
            {
                // читаю из того же файла, что и jar файл
                var jsonFile = Path.ChangeExtension(path, "json");
                var json = JObject.Parse(File.ReadAllText(jsonFile));

                return new Tuple<string, Version>(json["id"].ToString(), Version.Parse(json["jar"].ToString()));
            }
            catch 
            {
                try
                {
                    var zip = ZipFile.OpenRead(path);
                    var all = zip.Entries.Where(x => x.Name.Contains("json"))
                        .OrderByDescending(x => x.FullName)
                        .ToList();
                    var jsonZip = all.FirstOrDefault();
                    if (jsonZip != null)
                    {
                        using (var stream = jsonZip.Open())
                        {
                            using (var reader = new StreamReader(stream))
                            {
                                var json = JObject.Parse(reader.ReadToEnd());
                                return new Tuple<string, Version>(json["id"].ToString(), Version.Parse(json["jar"].ToString()));
                            }
                        }
                    }
                }
                catch 
                {
                    // ignored
                }
            }

            return new Tuple<string, Version>(string.Empty, GetVersion(Path.GetFileName(path)));
        }
        #endregion
    }
}
