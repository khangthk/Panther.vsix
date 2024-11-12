using Microsoft.Win32;
using System;

namespace Panther.Helpers
{
    internal static class RegistryHelper
    {
        private const string RegKeyPath = @"Software\Prowess\Panther\Extension";
        private const string RegNameAutoDeleteLockFiles = "AutoDeleteLockFiles";
        private const string RegNameAutoFixAppClosing = "AutoFixAppClosing";
        private const string RegNameAutoUnlockUtility = "AutoUnlockUtility";
        private const string RegNameDstPathList = "DstPathList";
        private const string RegNameDstPathSelected = "DstPathSelected";

        public static bool GetAutoDeleteLockFiles(bool defaultValue = true)
        {
            using (RegistryKey key = Registry.CurrentUser.OpenSubKey(RegKeyPath))
            {
                return (key?.GetValue(RegNameAutoDeleteLockFiles) as int? ?? (defaultValue ? 1 : 0)) == 1;
            }
        }

        public static void SaveAutoDeleteLockFiles(bool enable)
        {
            using (RegistryKey key = Registry.CurrentUser.CreateSubKey(RegKeyPath))
            {
                key?.SetValue(RegNameAutoDeleteLockFiles, enable ? 1 : 0);
            }
        }

        public static bool GetAutoFixAppClosing(bool defaultValue = true)
        {
            using (RegistryKey key = Registry.CurrentUser.OpenSubKey(RegKeyPath))
            {
                return (key?.GetValue(RegNameAutoFixAppClosing) as int? ?? (defaultValue ? 1 : 0)) == 1;
            }
        }

        public static void SaveAutoFixAppClosing(bool enable)
        {
            using (RegistryKey key = Registry.CurrentUser.CreateSubKey(RegKeyPath))
            {
                key?.SetValue(RegNameAutoFixAppClosing, enable ? 1 : 0);
            }
        }

        public static bool GetAutoUnlockUtility(bool defaultValue = true)
        {
            using (RegistryKey key = Registry.CurrentUser.OpenSubKey(RegKeyPath))
            {
                return (key?.GetValue(RegNameAutoUnlockUtility) as int? ?? (defaultValue ? 1 : 0)) == 1;
            }
        }

        public static void SaveAutoUnlockUtility(bool enable)
        {
            using (RegistryKey key = Registry.CurrentUser.CreateSubKey(RegKeyPath))
            {
                key?.SetValue(RegNameAutoUnlockUtility, enable ? 1 : 0);
            }
        }

        public static string[] GetDstPathList()
        {
            using (RegistryKey key = Registry.CurrentUser.OpenSubKey(RegKeyPath))
            {
                return key?.GetValue(RegNameDstPathList, Array.Empty<string>()) as string[] ?? Array.Empty<string>();
            }
        }

        public static void SaveDstPathList(string[] paths)
        {
            using (RegistryKey key = Registry.CurrentUser.CreateSubKey(RegKeyPath))
            {
                key?.SetValue(RegNameDstPathList, paths, RegistryValueKind.MultiString);
            }
        }

        public static string GetDstPathSelected()
        {
            using (RegistryKey key = Registry.CurrentUser.OpenSubKey(RegKeyPath))
            {
                return key?.GetValue(RegNameDstPathSelected, string.Empty)?.ToString() ?? string.Empty;
            }
        }

        public static void SaveDstPathSelected(string path)
        {
            using (RegistryKey key = Registry.CurrentUser.CreateSubKey(RegKeyPath))
            {
                key?.SetValue(RegNameDstPathSelected, path);
            }
        }
    }
}
