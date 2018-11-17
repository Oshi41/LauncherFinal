using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Exception = System.Exception;

namespace LauncherFinal.Helper
{
    public static class PathsHelper
    {
        public static int GetJumpsToRoot(string path)
        {
            try
            {
                // это и есть корень
                if (string.Equals(Path.GetPathRoot(path), path))
                    return 0;

                // файл прям в корне
                if (string.Equals(Path.GetPathRoot(path), Path.GetDirectoryName(path)))
                    return 1;

                return 1 + GetJumpsToRoot(Path.GetDirectoryName(path));
            }
            catch (Exception e)
            {
                Trace.Write(e);
                return int.MaxValue;
            }
        }

        public static string GetTopPath(IEnumerable<string> paths)
        {
            if (paths.IsNullOrEmpty())
            {
                return string.Empty;
            }

            if (paths.Count() == 1)
            {
                return paths.FirstOrDefault();
            }

            var map = paths.Distinct().ToDictionary(x => x, GetJumpsToRoot);
            var min = map.Min(x => x.Value);

            var values = map.Where(x => x.Value == min).ToList();
            return GetShortestName(values.Select(x => x.Key));
        }

        public static string GetShortestName(IEnumerable<string> paths)
        {
            if (paths.IsNullOrEmpty())
            {
                return string.Empty;
            }

            if (paths.Count() == 1)
            {
                return paths.FirstOrDefault();
            }

            var map = paths.Distinct().ToDictionary(x => x, GetNameLength);
            var min = map.Where(x => x.Value > 0).Min(x => x.Value);
            var val = map.FirstOrDefault(x => x.Value == min).Key ?? string.Empty;
            return val;
        }

        private static int GetNameLength(string fullPath)
        {
            try
            {
                // ReSharper disable once PossibleNullReferenceException
                return Path.GetFileName(fullPath).Length;
            }
            catch (Exception e)
            {
                Trace.Write(e);
                return -1;
            }
        }

        public static bool PlacedInFolder(string path, string folderName, 
            StringComparison comparison = StringComparison.Ordinal)
        {
            try
            {
                while (!string.IsNullOrEmpty(path))
                {
                    if (string.Equals(Path.GetFileName(path), folderName, comparison))
                        return true;

                    path = Path.GetDirectoryName(path);
                }

                return false;
            }
            catch (Exception e)
            {
                Trace.Write(e);
                return false;
            }
        }
    }
}
