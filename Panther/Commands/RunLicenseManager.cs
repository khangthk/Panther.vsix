namespace Panther.Commands
{
    internal sealed class RunLicenseManager : RunCommandBase
    {
        public RunLicenseManager() : base(Exe.LicenseManager) { }

        public override string GetText() => MenuTexts.RunLicenseManager;
    }
}
