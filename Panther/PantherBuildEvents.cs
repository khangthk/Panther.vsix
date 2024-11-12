using EnvDTE;
using Microsoft.VisualStudio.Shell;
using Panther.Helpers;
using System.Diagnostics;
using Task = System.Threading.Tasks.Task;

namespace Panther
{
    internal class PantherBuildEvents
    {
        public static PantherBuildEvents Instance { get; private set; }
        public bool IsBuilding { get; private set; }

        private PantherBuildEvents()
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            var dte = PackageHelper.GetDTE();
            dte.Events.BuildEvents.OnBuildBegin += OnBuildBegin;
            dte.Events.BuildEvents.OnBuildDone += OnBuildDone;
        }

        public static async Task InitializeAsync(AsyncPackage package)
        {
            await package.JoinableTaskFactory.SwitchToMainThreadAsync(package.DisposalToken);

            Instance = new PantherBuildEvents();
        }

        private void OnBuildBegin(vsBuildScope Scope, vsBuildAction Action)
        {
            Debug.WriteLine("--> OnBuildBegin event triggered.");

            IsBuilding = true;
        }

        private void OnBuildDone(vsBuildScope Scope, vsBuildAction Action)
        {
            Debug.WriteLine("--> OnBuildDone event triggered.");

            IsBuilding = false;
        }
    }
}
