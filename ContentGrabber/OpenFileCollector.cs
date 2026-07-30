using System;
using System.Collections.Generic;
using System.IO;

using EnvDTE;
using EnvDTE80;

using Microsoft.VisualStudio.Shell;

namespace ContentGrabber
{
	class OpenFileCollector
	{
		readonly OpenDocumentContentReader contentReader
			= new OpenDocumentContentReader();

		readonly ProjectPathResolver projectPathResolver
			= new ProjectPathResolver();

		public SelectedFile GetActiveFile(DTE2 dte)
		{
			ThreadHelper.ThrowIfNotOnUIThread();

			Document document = dte.ActiveDocument;
			if (document == null)
			{
				return null;
			}

			return TryCreateFile(document, out SelectedFile file)
				? file
				: null;
		}

		public IReadOnlyList<SelectedFile> GetOpenFiles(DTE2 dte)
		{
			ThreadHelper.ThrowIfNotOnUIThread();

			var results = new List<SelectedFile>();
			var seen = new HashSet<string>(
				StringComparer.OrdinalIgnoreCase);

			foreach (Document document in dte.Documents)
			{
				if (!TryCreateFile(document, out SelectedFile file))
				{
					continue;
				}

				if (seen.Add(file.FullPath))
				{
					results.Add(file);
				}
			}

			return results;
		}

		bool TryCreateFile(Document document, out SelectedFile file)
		{
			ThreadHelper.ThrowIfNotOnUIThread();

			file = null;

			string filePath;

			try
			{
				filePath = document.FullName;
			}
			catch
			{
				return false;
			}

			if (string.IsNullOrWhiteSpace(filePath))
			{
				return false;
			}

			if (!File.Exists(filePath))
			{
				return false;
			}

			string content = contentReader.TryGetContent(document);
			if (content == null)
			{
				return false;
			}

			string displayPath
				= GetDisplayPathForOpenDocument(document, filePath);

			file = new SelectedFile(filePath, displayPath, content);

			return true;
		}

		string GetDisplayPathForOpenDocument(
			Document document,
			string filePath)
		{
			ThreadHelper.ThrowIfNotOnUIThread();

			try
			{
				ProjectItem projectItem = document.ProjectItem;

				if (projectItem != null)
				{
					string projectRelativePath
						= projectPathResolver.TryGetProjectRelativePath(projectItem);

					if (!string.IsNullOrWhiteSpace(projectRelativePath))
					{
						return projectRelativePath;
					}
				}
			}
			catch
			{
				/* Fall back to the file name for non-project documents. */
			}

			return Path.GetFileName(filePath);
		}
	}
}