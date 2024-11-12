using Panther.Helpers;
using System.IO;
using System.Threading.Tasks;

namespace Panther.Commands
{
    internal abstract class BuildSolutionBase : BuildTargetBase
    {
        protected bool IsRebuild = false;

        protected override void OnCommandDone()
        {
            _ = OnCopyWhenDoneAsync();
            PrintCompleted();
        }

        protected async Task CopyFolderAsync(string solutionFilePath, string folder, string dstPath)
        {
            var srcPath = Path.Combine(Directory.GetParent(solutionFilePath).FullName, folder);
            await FileFolderHelper.CopyFolderAsync(srcPath, dstPath);
        }

        protected abstract Task OnCopyWhenDoneAsync();
    }
}
