namespace Panther.Commands
{
    internal sealed class RunSecurityManager : RunCommandBase
    {
        public RunSecurityManager() : base(Exe.SecurityManager) { }

        public override string GetText() => MenuTexts.RunSecurityManager;
    }
}
