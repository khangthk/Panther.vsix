namespace Panther.Commands
{
    internal sealed class RunLicenseServer : RunCommandBase
    {
        public RunLicenseServer() : base(Exe.LicenseServer) { }

        public override string GetText() => MenuTexts.RunLicenseServer;
    }
}
