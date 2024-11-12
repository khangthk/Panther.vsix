namespace Panther.Commands
{
    internal sealed class RunWTCP : RunCommandBase
    {
        public RunWTCP() : base(Exe.WTCP) { }

        public override string GetText() => MenuTexts.RunWTCP;
    }
}
