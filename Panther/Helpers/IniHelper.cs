using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Panther.Helpers
{
    internal static class IniHelper
    {
        public static bool DeleteSection(string filePath, string section)
        {
            if (File.Exists(filePath))
            {
                var newLines = new List<string>();
                var inSection = false;
                var needSave = false;

                foreach (var line in File.ReadAllLines(filePath))
                {
                    if (line.Trim().IndexOf(section, StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        inSection = true;
                        needSave = true;
                        continue;
                    }

                    if (inSection && line.StartsWith("[") && line.EndsWith("]"))
                    {
                        inSection = false;
                    }

                    if (!inSection)
                    {
                        newLines.Add(line);
                    }
                }

                if (needSave)
                {
                    try
                    {
                        File.WriteAllLines(filePath, newLines);
                    }
                    catch (Exception)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public static string GetIniValue(string filePath, string section, string key)
        {
            if (!File.Exists(filePath))
            {
                return string.Empty;
            }

            var inSection = false;

            foreach (var line in File.ReadLines(filePath))
            {
                if (line.Trim().IndexOf(section, StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    inSection = true;
                    continue;
                }

                if (inSection)
                {
                    if (line.StartsWith("[") && line.EndsWith("]"))
                    {
                        break;
                    }

                    var match = Regex.Match(line, @"^(\w+)=([\s\S]*)$");
                    if (match.Success && match.Groups[1].Value.Equals(key, StringComparison.OrdinalIgnoreCase))
                    {
                        return match.Groups[2].Value.Trim();
                    }
                }
            }

            return string.Empty;
        }

        public static Dictionary<string, string> GetIniValue(string filePath, string section, string[] keys)
        {
            var result = keys.ToDictionary(key => key, key => string.Empty);

            if (!File.Exists(filePath))
            {
                return result;
            }

            var inSection = false;

            foreach (var line in File.ReadLines(filePath))
            {
                if (line.Trim().IndexOf(section, StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    inSection = true;
                    continue;
                }

                if (inSection)
                {
                    if (line.StartsWith("[") && line.EndsWith("]"))
                    {
                        break;
                    }

                    var match = Regex.Match(line, @"^(\w+)=([\s\S]*)$");
                    foreach (var key in keys)
                    {
                        if (match.Success && match.Groups[1].Value.Equals(key, StringComparison.OrdinalIgnoreCase))
                        {
                            result[key] = match.Groups[2].Value.Trim();
                        }
                    }
                }
            }

            return result;
        }
    }
}
