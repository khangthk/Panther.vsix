﻿using Microsoft.VisualStudio.Shell;
using Panther.Commands;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Diagnostics;
using Task = System.Threading.Tasks.Task;

namespace Panther
{
    internal sealed class PantherCommands
    {
        public static readonly Guid CommandSet = PackageGuids.guidPantherPackageCmdSet;

        private readonly OleMenuCommandService _commandService;
        private readonly Dictionary<int, CommandBase> _commands = new Dictionary<int, CommandBase>();

        private PantherCommands(OleMenuCommandService commandService)
        {
            _commandService = commandService ?? throw new ArgumentNullException(nameof(commandService));

            try
            {
                RegisterCommands();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"--> Error initializing PantherCommands: {ex.Message}");
            }
        }

        public static PantherCommands Instance { get; private set; }

        public static async Task InitializeAsync(AsyncPackage package)
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(package.DisposalToken);

            var commandService = await package.GetServiceAsync(typeof(IMenuCommandService)) as OleMenuCommandService;
            Instance = new PantherCommands(commandService);
        }

        private void Execute(object sender, EventArgs e)
        {
            var menuCommand = (OleMenuCommand)sender;
            if (_commands.TryGetValue(menuCommand.CommandID.ID, out var command))
            {
                Debug.WriteLine($"--> Execute {command.GetType().FullName}");
                _ = command.OnCommandAsync(sender, e);
            }
        }

        private void RegisterCommands()
        {
            RegisterCommand(new BuildDependencies(), PackageIds.CommandBuildDependencies);
            RegisterCommand(new BuildThirdParty(), PackageIds.CommandBuildThirdParty);
            RegisterCommand(new BuildPantherCommon(), PackageIds.CommandBuildPantherCommon);
            RegisterCommand(new RebuildPantherCommon(), PackageIds.CommandRebuildPantherCommon);
            RegisterCommand(new BuildCoreCommon(), PackageIds.CommandBuildCoreCommon);
            RegisterCommand(new RebuildCoreCommon(), PackageIds.CommandRebuildCoreCommon);
            RegisterCommand(new RunPantherTPS(), PackageIds.CommandRunPantherTPS);
            RegisterCommand(new RunConfigTool(), PackageIds.CommandRunConfigTool);
            RegisterCommand(new RunMachineDataEntry(), PackageIds.CommandRunMachineDataEntry);
            RegisterCommand(new RunImportTool(), PackageIds.CommandRunImportTool);
            RegisterCommand(new RunAMSManager(), PackageIds.CommandRunAMSManager);
            RegisterCommand(new RunLicenseManager(), PackageIds.CommandRunLicenseManager);
            RegisterCommand(new RunLicenseServer(), PackageIds.CommandRunLicenseServer);
            RegisterCommand(new RunSecurityManager(), PackageIds.CommandRunSecurityManager);
            RegisterCommand(new RunDICOMCommunicator(), PackageIds.CommandRunDICOMCommunicator);
            RegisterCommand(new RunDICOMServer(), PackageIds.CommandRunDICOMServer);
            RegisterCommand(new RunWTCP(), PackageIds.CommandRunWTCP);
            RegisterCommand(new RunLoggingTool(), PackageIds.CommandRunLoggingTool);
            RegisterCommand(new CopyBinFiles(), PackageIds.CommandCopyBinFiles);
            RegisterCommand(new OpenBinFolder(), PackageIds.CommandOpenBinFolder);
            RegisterCommand(new DeleteLockFilesInPantherSite(), PackageIds.CommandDeleteLockFilesInPantherSite);
            RegisterCommand(new UnlockUtilityLockedByUser(), PackageIds.CommandUnlockUtilityLockedByUser);
            RegisterCommand(new FixAppClosingAfter15Seconds(), PackageIds.CommandFixAppClosingAfter15Seconds);
            RegisterCommand(new EnableDisableAutoDeleteLockFiles(), PackageIds.CommandEnableDisableAutoDeleteLockFiles);
            RegisterCommand(new EnableDisableAutoUnlockUtility(), PackageIds.CommandEnableDisableAutoUnlockUtility);
            RegisterCommand(new EnableDisableAutoFixAppClosing(), PackageIds.CommandEnableDisableAutoFixAppClosing);
            RegisterCommand(new SettingComponents(), PackageIds.CommandSettingComponents);
        }

        private void RegisterCommand(CommandBase command, int id)
        {
            var menuCommandID = new CommandID(CommandSet, id);
            var menuCommand = new OleMenuCommand(Execute, menuCommandID);
            menuCommand.BeforeQueryStatus += OnBeforeQueryStatus;
            _commandService.AddCommand(menuCommand);
            _commands[id] = command;
        }

        private void OnBeforeQueryStatus(object sender, EventArgs e)
        {
            var menuCommand = (OleMenuCommand)sender;
            if (_commands.TryGetValue(menuCommand.CommandID.ID, out var command))
            {
                menuCommand.Checked = command.IsChecked();
                menuCommand.Enabled = command.IsEnabled();
                menuCommand.Text = (command.IsExecuting ? "=>" : "") + command.GetText();
            }
        }
    }
}
