using System;
using System.ComponentModel.Design;
using System.Threading.Tasks;
using System.Windows;

using EnvDTE80;

using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace ContentGrabber
{
	sealed class CopyOpenFileAsMarkdownCommand
	{
		public const int CommandId = 0x0101;

		public static readonly Guid CommandSet
			= new Guid("3d6c9c9e-6b6a-4a1e-9f1a-2f4c6f9b2c11");

		readonly AsyncPackage package;

		CopyOpenFileAsMarkdownCommand(AsyncPackage package,
									  OleMenuCommandService commandService)
		{
			this.package = package
						   ?? throw new ArgumentNullException(nameof(package));

			var commandId = new CommandID(CommandSet, CommandId);
			var menuItem = new OleMenuCommand(Execute, commandId);

			menuItem.BeforeQueryStatus += OnBeforeQueryStatus;

			commandService.AddCommand(menuItem);
		}

		public static async Task InitializeAsync(AsyncPackage package)
		{
			await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

			var commandService
				= await package.GetServiceAsync(
					typeof(IMenuCommandService)) as OleMenuCommandService;

			if (commandService == null)
			{
				throw new InvalidOperationException(
					"Unable to get menu command service.");
			}

			_ = new CopyOpenFileAsMarkdownCommand(package, commandService);
		}

		void OnBeforeQueryStatus(object sender, EventArgs e)
		{
			ThreadHelper.ThrowIfNotOnUIThread();

			var menuCommand = sender as OleMenuCommand;

			if (menuCommand == null)
			{
				return;
			}

			DTE2 dte = GetDte();

			menuCommand.Visible = true;
			menuCommand.Enabled = dte != null && dte.ActiveDocument != null;
		}

		void Execute(object sender, EventArgs e)
		{
			ThreadHelper.ThrowIfNotOnUIThread();

			try
			{
				DTE2 dte = GetDte();
				if (dte == null)
				{
					ShowMessage("Unable to access Visual Studio services.");

					return;
				}

				var collector = new OpenFileCollector();
				SelectedFile file = collector.GetActiveFile(dte);

				if (file == null)
				{
					ShowMessage("The active document is not a readable text file.");

					return;
				}

				var formatter = new MarkdownFileFormatter();

				string markdown = formatter.BuildMarkdown(
					new[] { file });

				if (string.IsNullOrWhiteSpace(markdown))
				{
					ShowMessage("No readable content was found.");

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

			return package
				.GetServiceAsync(typeof(SDTE))
				.ConfigureAwait(false)
				.GetAwaiter()
				.GetResult() as DTE2;
		}

		void ShowMessage(string message)
		{
			ThreadHelper.ThrowIfNotOnUIThread();

			VsShellUtilities.ShowMessageBox(
				package,
				message,
				"ContentGrabber",
				OLEMSGICON.OLEMSGICON_INFO,
				OLEMSGBUTTON.OLEMSGBUTTON_OK,
				OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST);
		}
	}
}