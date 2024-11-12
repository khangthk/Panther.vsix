using Microsoft.VisualStudio.Shell;
using Panther.Helpers;
using System.IO;
using Task = System.Threading.Tasks.Task;

namespace Panther.Commands
{
    internal class BuildPantherCommon : BuildSolutionBase
    {
        protected override async Task ExecuteAsync()
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

            var solutionFilePath = PantherHelper.GetPantherCommonSolutionFilePath();
            await MSBuildHelper.BuildSolutionAsync(solutionFilePath, Configuration.Debug, IsRebuild);
            await MSBuildHelper.BuildSolutionAsync(solutionFilePath, Configuration.Release, IsRebuild);
        }

        protected override async Task OnCopyWhenDoneAsync()
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

            var solutionFilePath = PantherHelper.GetPantherCommonSolutionFilePath();
            await CopyFolderAsync(solutionFilePath, "Bin", PantherHelper.GetBinDirPath());
            await CopyFolderAsync(solutionFilePath, "Lib", Path.Combine(PantherHelper.GetExternalsDirPath(), "PantherCommon", "Lib"));
            await CopyFolderAsync(solutionFilePath, Path.Combine("Include", "Base"), Path.Combine(PantherHelper.GetExternalsDirPath(), "PantherCommon", "Include", "Base"));
        }

        public override string GetText() => MenuTexts.BuildPantherCommon;

        public override bool IsEnabled()
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            return base.IsEnabled() && !PantherBuildEvents.Instance.IsBuilding &&
                   File.Exists(PantherHelper.GetPantherCommonSolutionFilePath());
        }
    }
}
