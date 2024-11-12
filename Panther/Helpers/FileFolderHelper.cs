using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace Panther.Helpers
{
    internal static class FileFolderHelper
    {
        public static void OpenFolderInExplorer(string dirPath)
        {
            if (Directory.Exists(dirPath))
            {
                Process.Start("explorer.exe", dirPath);
            }
        }

        public static async Task<(int success, int failed)> DeleteFileAsync(string dirPath, string deleteFileName, bool recursive = true, bool showResultInOutputPane = true)
        {
            int successCount = 0;
            int failedCount = 0;

            try
            {
                foreach (var file in Directory.EnumerateFiles(dirPath, "*", SearchOption.TopDirectoryOnly))
                {
                    var fileName = Path.GetFileName(file);
                    if (fileName.Equals(deleteFileName, StringComparison.OrdinalIgnoreCase))
                    {
                        try
                        {
                            File.Delete(file);
                            successCount++;

                            if (showResultInOutputPane)
                            {
                                await OutputPaneHelper.OutputStringAsync($"[OK] {file}");
                            }
                        }
                        catch (Exception ex)
                        {
                            failedCount++;

                            if (showResultInOutputPane)
                            {
                                await OutputPaneHelper.OutputStringAsync($"[ERROR] {file}: {ex.Message}");
                            }
                        }
                    }
                }

                if (recursive)
                {
                    var subDirs = Directory.EnumerateDirectories(dirPath, "*", SearchOption.TopDirectoryOnly);
                    var deleteTasks = await Task.WhenAll(subDirs.Select(dir => DeleteFileAsync(dir, deleteFileName, recursive, showResultInOutputPane)));

                    successCount += deleteTasks.Sum(result => result.success);
                    failedCount += deleteTasks.Sum(result => result.failed);
                }
            }
            catch (Exception ex)
            {
                if (showResultInOutputPane)
                {
                    await OutputPaneHelper.OutputStringAsync($"[ERROR]: {ex.Message}");
                }
            }

            return (successCount, failedCount);
        }

        public static async Task CopyFolderAsync(string srcDirPath, string dstDirPath, bool copyOnlyDiffFiles = true, bool showResultInOutputPane = true)
        {
            var srcDir = new DirectoryInfo(srcDirPath);
            var dstDir = new DirectoryInfo(dstDirPath);

            try
            {
                dstDir.Create();
            }
            catch (Exception ex)
            {
                if (showResultInOutputPane)
                {
                    await OutputPaneHelper.OutputStringAsync($"[ERROR] {ex.Message}");
                }
            }

            if (!srcDir.Exists || !dstDir.Exists)
            {
                return;
            }

            foreach (var srcFile in srcDir.GetFiles("*", SearchOption.AllDirectories))
            {
                var relativePath = srcFile.FullName.Substring(srcDir.FullName.Length + 1);
                var dstFilePath = Path.Combine(dstDir.FullName, relativePath);

                if (!File.Exists(dstFilePath) || !copyOnlyDiffFiles || !FilesAreEqual(srcFile.FullName, dstFilePath))
                {
                    try
                    {
                        Directory.CreateDirectory(Path.GetDirectoryName(dstFilePath));
                        srcFile.CopyTo(dstFilePath, true);

                        if (showResultInOutputPane)
                        {
                            await OutputPaneHelper.OutputStringAsync($"[OK] {relativePath} => {dstFilePath}");
                        }
                    }
                    catch (Exception ex)
                    {
                        if (showResultInOutputPane)
                        {
                            await OutputPaneHelper.OutputStringAsync($"[ERROR] {relativePath} => {ex.Message}");
                        }
                    }
                }
            }
        }

        private static bool FilesAreEqual(string filePath1, string filePath2)
        {
            byte[] fileHash1 = GetFileHash(filePath1);
            byte[] fileHash2 = GetFileHash(filePath2);

            return StructuralComparisons.StructuralEqualityComparer.Equals(fileHash1, fileHash2);
        }

        private static byte[] GetFileHash(string filePath)
        {
            using (var md5 = MD5.Create())
            using (var stream = File.OpenRead(filePath))
            {
                return md5.ComputeHash(stream);
            }
        }
    }
}
