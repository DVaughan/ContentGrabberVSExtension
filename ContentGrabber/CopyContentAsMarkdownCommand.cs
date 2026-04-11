using System;
using System.ComponentModel.Design;
using System.Threading.Tasks;
using System.Windows;
using EnvDTE80;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace ContentGrabber
{
	sealed class CopyContentAsMarkdownCommand
	{
		public const int CommandId = 0x0100;
		public static readonly Guid CommandSet = new Guid("3d6c9c9e-6b6a-4a1e-9f1a-2f4c6f9b2c11");

		readonly AsyncPackage package;

		CopyContentAsMarkdownCommand(AsyncPackage package, OleMenuCommandService commandService)
		{
			this.package = package ?? throw new ArgumentNullException(nameof(package));

			var commandId = new CommandID(CommandSet, CommandId);
			var menuItem = new OleMenuCommand(Execute, commandId);
			menuItem.BeforeQueryStatus += OnBeforeQueryStatus;

			commandService.AddCommand(menuItem);
		}

		public static async Task InitializeAsync(AsyncPackage package)
		{
			await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

			var commandService = await package.GetServiceAsync(typeof(IMenuCommandService)) as OleMenuCommandService;
			if (commandService == null)
			{
				throw new InvalidOperationException("Unable to get menu command service.");
			}

			_ = new CopyContentAsMarkdownCommand(package, commandService);
		}

		void OnBeforeQueryStatus(object sender, EventArgs e)
		{
			ThreadHelper.ThrowIfNotOnUIThread();

			var menuCommand = sender as OleMenuCommand;
			if (menuCommand == null)
			{
				return;
			}

			menuCommand.Visible = true;
			menuCommand.Enabled = HasSelectedFiles();
		}

		bool HasSelectedFiles()
		{
			ThreadHelper.ThrowIfNotOnUIThread();

			var dte = GetDte();
			if (dte == null)
			{
				return false;
			}

			var collector = new SelectedFileCollector();
			var selectedFiles = collector.GetSelectedFiles(dte);

			return selectedFiles.Count > 0;
		}

		void Execute(object sender, EventArgs e)
		{
			ThreadHelper.ThrowIfNotOnUIThread();

			try
			{
				var dte = GetDte();
				if (dte == null)
				{
					ShowMessage("Unable to access Visual Studio services.");
					return;
				}

				var collector = new SelectedFileCollector();
				var formatter = new MarkdownFileFormatter();

				var selectedFiles = collector.GetSelectedFiles(dte);
				if (selectedFiles.Count == 0)
				{
					ShowMessage("No files were selected.");
					return;
				}

				string markdown = formatter.BuildMarkdown(selectedFiles);

				if (string.IsNullOrWhiteSpace(markdown))
				{
					ShowMessage("No readable files were found.");
					return;
				}

				Clipboard.SetText(markdown);
			}
			catch (Exception ex)
			{
				ShowMessage("ContentGrabber failed:\r\n\r\n" + ex.Message);
			}
		}

		DTE2 GetDte()
		{
			ThreadHelper.ThrowIfNotOnUIThread();

			return package.GetServiceAsync(typeof(SDTE)).ConfigureAwait(false).GetAwaiter().GetResult() as DTE2;
		}

		void ShowMessage(string message)
		{
			ThreadHelper.ThrowIfNotOnUIThread();

			VsShellUtilities.ShowMessageBox(
				package,
				message,
				"ContentGrabber",
				Microsoft.VisualStudio.Shell.Interop.OLEMSGICON.OLEMSGICON_INFO,
				Microsoft.VisualStudio.Shell.Interop.OLEMSGBUTTON.OLEMSGBUTTON_OK,
				Microsoft.VisualStudio.Shell.Interop.OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST);
		}
	}
}