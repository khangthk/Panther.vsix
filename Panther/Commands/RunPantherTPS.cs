namespace Panther.Commands
{
    internal sealed class RunPantherTPS : RunCommandBase
    {
        public RunPantherTPS() : base(Exe.PantherTPS) { }

        public override string GetText() => MenuTexts.RunPantherTPS;
    }
}
