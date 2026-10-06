using System;
using System.Collections.Generic;
using System.IO;

using EnvDTE;

using EnvDTE80;

using Microsoft.VisualStudio.Shell;

namespace ContentGrabber
{
	class SelectedFileCollector
	{
		readonly ProjectPathResolver projectPathResolver = new ProjectPathResolver();

		public IReadOnlyList<SelectedFile> GetSelectedFiles(DTE2 dte)
		{
			ThreadHelper.ThrowIfNotOnUIThread();

			List<SelectedFile> results = new List<SelectedFile>();
			HashSet<string> seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

			foreach (SelectedItem selectedItem in dte.SelectedItems)
			{
				ProjectItem projectItem = selectedItem.ProjectItem;
				if (projectItem != null)
				{
					TryAddProjectItemFiles(projectItem, results, seen);
					continue;
				}

				Project project = selectedItem.Project;
				if (project != null && !string.IsNullOrWhiteSpace(project.FullName))
				{
					string filePath = project.FullName;
					string displayPath = Path.GetFileName(filePath);

					TryAddFile(filePath,
						displayPath,
						project.Name,
						results,
						seen);
				}
			}

			return results;
		}

		void TryAddProjectItemFiles(ProjectItem projectItem,
									List<SelectedFile> results,
									HashSet<string> seen)
		{
			ThreadHelper.ThrowIfNotOnUIThread();

			try
			{
				short fileCount = projectItem.FileCount;

				for (short index = 1; index <= fileCount; index++)
				{
					string filePath = projectItem.FileNames[index];
					string displayPath = GetDisplayPath(projectItem, filePath);
					string projectName = projectItem.ContainingProject?.Name;

					TryAddFile(
						filePath,
						displayPath,
						projectName,
						results,
						seen);
				}
			}
			catch
			{
				/* Ignore unsupported item types. */
			}
		}

		void TryAddFile(string filePath,
						string displayPath,
						string projectName,
						List<SelectedFile> results,
						HashSet<string> seen)
		{
			if (string.IsNullOrWhiteSpace(filePath))
			{
				return;
			}

			if (Directory.Exists(filePath))
			{
				return;
			}

			if (!File.Exists(filePath))
			{
				return;
			}

			if (seen.Add(filePath))
			{
				results.Add(
					new SelectedFile(filePath, displayPath, projectName));
			}
		}

		string GetDisplayPath(ProjectItem projectItem, string filePath)
		{
			ThreadHelper.ThrowIfNotOnUIThread();

			string projectRelativePath
				= projectPathResolver.TryGetProjectRelativePath(projectItem);

			if (!string.IsNullOrWhiteSpace(projectRelativePath))
			{
				return projectRelativePath;
			}

			return Path.GetFileName(filePath);
		}
	}
}