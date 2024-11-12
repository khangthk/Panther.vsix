using Panther.Helpers;
using Task = System.Threading.Tasks.Task;

namespace Panther.Commands
{
    internal class BuildDependencies : BuildTargetBase
    {
        protected override async Task ExecuteAsync()
        {
            await MSBuildHelper.BuildTargetAsync(Target.Dependencies);
        }

        public override string GetText() => MenuTexts.BuildDependencies;
    }
}
