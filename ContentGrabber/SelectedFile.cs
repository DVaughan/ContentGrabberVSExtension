using System;

namespace ContentGrabber
{
	class SelectedFile
	{
		public SelectedFile(string fullPath, string displayPath)
		{
			FullPath    = fullPath    ?? throw new ArgumentNullException(nameof(fullPath));
			DisplayPath = displayPath ?? throw new ArgumentNullException(nameof(displayPath));
		}

		public string FullPath { get; }

		public string DisplayPath { get; }
	}
}