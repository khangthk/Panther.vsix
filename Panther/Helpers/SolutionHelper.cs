using EnvDTE;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Panther.Helpers
{
    internal static class SolutionHelper
    {
        public static bool SolutionExists()
        {
            return PackageHelper.GetDTE()?.Solution?.FullName?.Length > 0;
        }

        public static string GetSolutionDirPath()
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            return SolutionExists() ? Directory.GetParent(PackageHelper.GetDTE().Solution.FullName)?.FullName : string.Empty;
        }

        public static string GetPropertyValueOfProject(Project project, string key)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            var value = string.Empty;

            if (project != null && !string.IsNullOrEmpty(key))
            {
                IVsHierarchy hierarchy = GetProjectHierarchy(project);

                if (hierarchy != null && hierarchy is IVsBuildPropertyStorage propertyStorage)
                {
                    propertyStorage.GetPropertyValue(key, null, (uint)_PersistStorageType.PST_PROJECT_FILE, out value);
                }
            }

            return value;
        }

        public static Dictionary<string, string> GetPropertyValueOfProject(Project project, string[] keys)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            var result = keys.ToDictionary(key => key, key => string.Empty);

            if (project != null && keys != null && keys.Any())
            {
                IVsHierarchy hierarchy = GetProjectHierarchy(project);

                if (hierarchy != null && hierarchy is IVsBuildPropertyStorage propertyStorage)
                {
                    foreach (var key in keys)
                    {
                        propertyStorage.GetPropertyValue(key, null, (uint)_PersistStorageType.PST_PROJECT_FILE, out string value);
                        result[key] = value;
                    }
                }
            }

            return result;
        }

        private static IVsHierarchy GetProjectHierarchy(Project project)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            if (PackageHelper.GetServiceProvider().GetService(typeof(SVsSolution)) is IVsSolution solution)
            {
                if (ErrorHandler.Succeeded(solution.GetProjectOfUniqueName(project.UniqueName, out IVsHierarchy hierarchy)))
                {
                    return hierarchy;
                }
            }

            return null;
        }
    }
}
