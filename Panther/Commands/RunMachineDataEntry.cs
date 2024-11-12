namespace Panther.Commands
{
    internal sealed class RunMachineDataEntry : RunCommandBase
    {
        public RunMachineDataEntry() : base(Exe.MachineDataEntry) { }

        public override string GetText() => MenuTexts.RunMachineDataEntry;
    }
}
