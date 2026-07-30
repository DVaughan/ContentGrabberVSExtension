using System;

namespace ContentGrabber
{
	class SelectedFile
	{
		public SelectedFile(string fullPath, string displayPath)
			: this(fullPath, displayPath, null)
		{
		}

		public SelectedFile(string fullPath,
							string displayPath,
							string currentContent)
		{
			FullPath       = fullPath
							 ?? throw new ArgumentNullException(nameof(fullPath));
			DisplayPath    = displayPath
							 ?? throw new ArgumentNullException(nameof(displayPath));

			CurrentContent = currentContent;
		}

		public string FullPath { get; }

		public string DisplayPath { get; }

		/*
		 * Null means that the formatter should read the file from disk.
		 * An empty string represents an open, empty document.
		 */
		public string CurrentContent { get; }
	}
}