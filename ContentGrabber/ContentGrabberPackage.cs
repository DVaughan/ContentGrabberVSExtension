using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Shell;

namespace ContentGrabber
{
	[PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
	[InstalledProductRegistration(
		"ContentGrabber",
		"Copy selected or open file contents as markdown", "1.1")]
	[ProvideMenuResource("Menus.ctmenu", 1)]
	[Guid(PackageGuidString)]
	public sealed class ContentGrabberPackage : AsyncPackage
	{
		public const string PackageGuidString = "f55a5380-74fc-4025-9261-b24846c27c20";

		protected override async Task InitializeAsync(
			CancellationToken token,
			IProgress<ServiceProgressData> progress)
		{
			await JoinableTaskFactory.SwitchToMainThreadAsync(token);

			await CopyContentAsMarkdownCommand.InitializeAsync(this);
			await CopyOpenFileAsMarkdownCommand.InitializeAsync(this);
			await CopyAllOpenFilesAsMarkdownCommand.InitializeAsync(this);
		}
	}
}