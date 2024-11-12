namespace Panther.Commands
{
    internal sealed class RunDICOMCommunicator : RunCommandBase
    {
        public RunDICOMCommunicator() : base(Exe.DICOMCommunicator) { }

        public override string GetText() => MenuTexts.RunDICOMCommunicator;
    }
}
