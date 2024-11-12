using Panther.Helpers;
using Task = System.Threading.Tasks.Task;

namespace Panther.Commands
{
    internal class BuildThirdParty : BuildTargetBase
    {
        protected override async Task ExecuteAsync()
        {
            await MSBuildHelper.BuildTargetAsync(Target.ThirdParty);
        }

        public override string GetText() => MenuTexts.BuildThirdParty;
    }
}
