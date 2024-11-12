using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Shell;
using System;
using Task = System.Threading.Tasks.Task;

namespace Panther.Helpers
{
    internal static class PackageHelper
    {
        private static AsyncPackage _package;
        private static IServiceProvider _serviceProvider;
        private static DTE2 _dte;

        public static AsyncPackage GetPackage() => _package;
        public static IServiceProvider GetServiceProvider() => _serviceProvider;
        public static DTE2 GetDTE() => _dte;

        public static async Task InitializeAsync(AsyncPackage package)
        {
            await package.JoinableTaskFactory.SwitchToMainThreadAsync(package.DisposalToken);

            _package = package;
            _serviceProvider = package;
            _dte = await package.GetServiceAsync(typeof(DTE)) as DTE2;
        }
    }
}
