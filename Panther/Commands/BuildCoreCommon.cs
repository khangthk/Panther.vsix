using Microsoft.VisualStudio.Shell;
using Panther.Helpers;
using System.IO;
using Task = System.Threading.Tasks.Task;

namespace Panther.Commands
{
    internal class BuildCoreCommon : BuildSolutionBase
    {
        protected override async Task ExecuteAsync()
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

            var solutionFilePath = PantherHelper.GetCoreCommonSolutionFilePath();
            await MSBuildHelper.BuildSolutionAsync(solutionFilePath, Configuration.Debug);
            await MSBuildHelper.BuildSolutionAsync(solutionFilePath, Configuration.Release);
        }

        protected override async Task OnCopyWhenDoneAsync()
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

            var solutionFilePath = PantherHelper.GetCoreCommonSolutionFilePath();
            await CopyFolderAsync(solutionFilePath, "Bin", PantherHelper.GetBinDirPath());
            await CopyFolderAsync(solutionFilePath, "Lib", Path.Combine(PantherHelper.GetExternalsDirPath(), "CoreCommon", "Lib"));
            await CopyFolderAsync(solutionFilePath, Path.Combine("Include", "Math"), Path.Combine(PantherHelper.GetExternalsDirPath(), "CoreCommon", "Include", "Math"));
        }

        public override string GetText() => MenuTexts.BuildCoreCommon;

        public override bool IsEnabled()
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            return base.IsEnabled() && !PantherBuildEvents.Instance.IsBuilding &&
                   File.Exists(PantherHelper.GetCoreCommonSolutionFilePath());
        }
    }
}
