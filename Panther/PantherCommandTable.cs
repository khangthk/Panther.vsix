using System;

namespace Panther
{
    internal static class PackageGuids
    {
        public const string guidPantherPackageString = "ea6b662e-71cd-4115-96cd-0076516b6475";
        public const string guidPantherPackageCmdSetString = "68d99c7e-3d4a-4d3d-ae44-5c7025d08e3a";
        public static readonly Guid guidPantherPackage = new Guid(guidPantherPackageString);
        public static readonly Guid guidPantherPackageCmdSet = new Guid(guidPantherPackageCmdSetString);
    }

    internal static class PackageIds
    {
        public const int PantherMenu = 0x1000;
        public const int MenuBuildGroup = 0x1001;
        public const int MenuRunGroup = 0x1002;
        public const int MenuBinGroup = 0x1003;
        public const int MenuUtilityGroup = 0x1004;
        public const int MenuSettingGroup = 0x1005;

        public const int CommandBuildDependencies = 0x1100;
        public const int CommandBuildThirdParty = 0x1101;
        public const int CommandBuildPantherCommon = 0x1102;
        public const int CommandRebuildPantherCommon = 0x1103;
        public const int CommandBuildCoreCommon = 0x1104;
        public const int CommandRebuildCoreCommon = 0x1105;

        public const int CommandRunPantherTPS = 0x1200;
        public const int CommandRunConfigTool = 0x1201;
        public const int CommandRunMachineDataEntry = 0x1202;
        public const int CommandRunImportTool = 0x1203;
        public const int CommandRunAMSManager = 0x1204;
        public const int CommandRunLicenseManager = 0x1205;
        public const int CommandRunLicenseServer = 0x1206;
        public const int CommandRunSecurityManager = 0x1207;
        public const int CommandRunDICOMCommunicator = 0x1208;
        public const int CommandRunDICOMServer = 0x1209;
        public const int CommandRunWTCP = 0x120A;
        public const int CommandRunLoggingTool = 0x120B;

        public const int CommandCopyBinFiles = 0x1300;
        public const int CommandOpenBinFolder = 0x1301;

        public const int CommandDeleteLockFilesInPantherSite = 0x1400;
        public const int CommandUnlockUtilityLockedByUser = 0x1401;
        public const int CommandFixAppClosingAfter15Seconds = 0x1402;
        public const int CommandEnableDisableAutoDeleteLockFiles = 0x1403;
        public const int CommandEnableDisableAutoUnlockUtility = 0x1404;
        public const int CommandEnableDisableAutoFixAppClosing = 0x1405;

        public const int CommandSettingComponents = 0x1500;
    }

    internal static class MenuTexts
    {
        public const string BuildDependencies = "Build Dependencies";
        public const string BuildThirdParty = "Build ThirdParty";
        public const string BuildPantherCommon = "Build PantherCommon";
        public const string RebuildPantherCommon = "Rebuild PantherCommon";
        public const string BuildCoreCommon = "Build CoreCommon";
        public const string RebuildCoreCommon = "Rebuild CoreCommon";
        public const string RunPantherTPS = "Run PantherTPS.exe";
        public const string RunConfigTool = "Run ConfigTool.exe";
        public const string RunMachineDataEntry = "Run MachineDataEntry.exe";
        public const string RunImportTool = "Run ImportTool.exe";
        public const string RunAMSManager = "Run AMSManager.exe";
        public const string RunLicenseManager = "Run LicenseManager.exe";
        public const string RunLicenseServer = "Run LicenseServer.exe";
        public const string RunSecurityManager = "Run SecurityManager.exe";
        public const string RunDICOMCommunicator = "Run DICOMCommunicator.exe";
        public const string RunDICOMServer = "Run DICOMServer.exe";
        public const string RunWTCP = "Run WTCP.exe";
        public const string RunLoggingTool = "Run LoggingTool.exe";
        public const string CopyBinFiles = "Copy Bin Files";
        public const string OpenBinFolder = "Open Bin Folder";
        public const string DeleteLockFilesInPantherSite = "Delete Lock Files in Panther Site";
        public const string UnlockUtilityLockedByUser = "Unlock Utility Locked By User";
        public const string FixAppClosingAfter15Seconds = "Fix App Closing After 15 Seconds";
        public const string EnableDisableAutoDeleteLockFiles = "Enable/Disable Auto Delete Lock Files";
        public const string EnableDisableAutoUnlockUtility = "Enable/Disable Auto Unlock Utility";
        public const string EnableDisableAutoFixAppClosing = "Enable/Disable Auto Fix App Closing";
        public const string SettingComponents = "Setting Components";
    }
}
