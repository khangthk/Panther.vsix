using EnvDTE;
using Microsoft.VisualStudio.Shell;
using System;
using System.Collections.Generic;
using System.IO;

namespace Panther.Helpers
{
    internal class PantherHelper
    {
        public static bool IsPantherSolution()
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            return GetProbdomProject() != null;
        }

        public static string GetPantherSolutionDirPath()
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            return IsPantherSolution() ? SolutionHelper.GetSolutionDirPath() : string.Empty;
        }

        public static string GetBinDirPath()
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            var solutionDirPath = GetPantherSolutionDirPath();
            return !string.IsNullOrEmpty(solutionDirPath) ? Path.Combine(solutionDirPath, "Bin") : string.Empty;
        }

        public static string GetConfigurationsDirPath()
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            var binDirPath = GetBinDirPath();
            return !string.IsNullOrEmpty(binDirPath) ? Path.Combine(binDirPath, "Configurations") : string.Empty;
        }

        public static string GetConfigurationsFileName()
        {
            return $"Host_{Environment.MachineName}.ini";
        }

        public static string GetConfigurationsFilePath()
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            var configurationsDirPath = GetConfigurationsDirPath();
            return !string.IsNullOrEmpty(configurationsDirPath) ? Path.Combine(configurationsDirPath, GetConfigurationsFileName()) : string.Empty;
        }

        public static string GetExternalsDirPath()
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            var solutionDirPath = GetPantherSolutionDirPath();
            return !string.IsNullOrEmpty(solutionDirPath) ? Path.Combine(solutionDirPath, "Externals") : string.Empty;
        }

        public static string GetPantherCommonSolutionFilePath()
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            var solutionDirPath = GetPantherSolutionDirPath();
            return !string.IsNullOrEmpty(solutionDirPath) ? Path.Combine(Directory.GetParent(solutionDirPath).FullName, "Common", "PantherCommon.sln") : string.Empty;
        }

        public static string GetCoreCommonSolutionFilePath()
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            var solutionDirPath = GetPantherSolutionDirPath();
            return !string.IsNullOrEmpty(solutionDirPath) ? Path.Combine(Directory.GetParent(solutionDirPath).FullName, "CoreCommon", "CoreCommon.sln") : string.Empty;
        }

        public static string GetPantherMSBuildTargetsFilePath()
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            var solutionDirPath = GetPantherSolutionDirPath();
            return !string.IsNullOrEmpty(solutionDirPath) ? Path.Combine(solutionDirPath, "Panther.MSBuild.Targets") : string.Empty;
        }

        public static string GetPantherComponentsDirName()
        {
            return "PantherComponentsDir";
        }

        public static string GetPantherComponentsDirPath()
        {
            var value = Environment.GetEnvironmentVariable(GetPantherComponentsDirName(), EnvironmentVariableTarget.User) ??
                        Environment.GetEnvironmentVariable(GetPantherComponentsDirName(), EnvironmentVariableTarget.Machine);

            if (Path.IsPathRooted(value))
            {
                value = value.TrimEnd(Path.DirectorySeparatorChar) + Path.DirectorySeparatorChar;
            }

            return value ?? string.Empty;
        }

        public static string GetProjectProperty(string propertyKey)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            return SolutionHelper.GetPropertyValueOfProject(GetProbdomProject(), propertyKey);
        }

        public static Dictionary<string, string> GetProjectProperty(string[] propertyKeys)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            return SolutionHelper.GetPropertyValueOfProject(GetProbdomProject(), propertyKeys);
        }

        private static Project GetProbdomProject()
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            if (SolutionHelper.SolutionExists())
            {
                var projects = PackageHelper.GetDTE().Solution.Projects;
                foreach (Project project in projects)
                {
                    if (project.Name.Equals("Probdom", StringComparison.OrdinalIgnoreCase))
                    {
                        return project;
                    }
                }
            }

            return null;
        }
    }
}
