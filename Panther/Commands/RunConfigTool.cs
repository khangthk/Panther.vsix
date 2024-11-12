namespace Panther.Commands
{
    internal sealed class RunConfigTool : RunCommandBase
    {
        public RunConfigTool() : base(Exe.ConfigTool) { }

        public override string GetText() => MenuTexts.RunConfigTool;
    }
}
