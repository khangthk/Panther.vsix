namespace Panther.Commands
{
    internal sealed class RunLoggingTool : RunCommandBase
    {
        public RunLoggingTool() : base(Exe.LoggingTool) { }

        public override string GetText() => MenuTexts.RunLoggingTool;
    }
}
