namespace Panther.Commands
{
    internal sealed class RunAMSManager : RunCommandBase
    {
        public RunAMSManager() : base(Exe.AMSManager) { }

        public override string GetText() => MenuTexts.RunAMSManager;
    }
}
