namespace Panther.Commands
{
    internal sealed class RunImportTool : RunCommandBase
    {
        public RunImportTool() : base(Exe.ImportTool) { }

        public override string GetText() => MenuTexts.RunImportTool;
    }
}
