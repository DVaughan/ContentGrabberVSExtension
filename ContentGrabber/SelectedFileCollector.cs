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
		public IReadOnlyList<SelectedFile> GetSelectedFiles(DTE2 dte)
		{
			ThreadHelper.ThrowIfNotOnUIThread();

			var results = new List<SelectedFile>();
			var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

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

					TryAddFile(filePath, displayPath, results, seen);
				}
			}

			return results;
		}

		void TryAddProjectItemFiles(ProjectItem projectItem, List<SelectedFile> results, HashSet<string> seen)
		{
			ThreadHelper.ThrowIfNotOnUIThread();

			try
			{
				short fileCount = projectItem.FileCount;
				for (short index = 1; index <= fileCount; index++)
				{
					string filePath = projectItem.FileNames[index];
					string displayPath = GetDisplayPath(projectItem, filePath);

					TryAddFile(filePath, displayPath, results, seen);
				}
			}
			catch
			{
				/* Ignore unsupported item types. */
			}
		}

		void TryAddFile(string filePath, string displayPath, List<SelectedFile> results, HashSet<string> seen)
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
				results.Add(new SelectedFile(filePath, displayPath));
			}
		}

		string GetDisplayPath(ProjectItem projectItem, string filePath)
		{
			ThreadHelper.ThrowIfNotOnUIThread();

			string projectRelativePath = TryGetProjectRelativePath(projectItem);
			if (!string.IsNullOrWhiteSpace(projectRelativePath))
			{
				return projectRelativePath;
			}

			return Path.GetFileName(filePath);
		}

		string TryGetProjectRelativePath(ProjectItem projectItem)
		{
			ThreadHelper.ThrowIfNotOnUIThread();

			var parts = new Stack<string>();
			ProjectItem currentItem = projectItem;

			while (currentItem != null)
			{
				if (string.IsNullOrWhiteSpace(currentItem.Name))
				{
					break;
				}

				parts.Push(currentItem.Name);
				currentItem = currentItem.Collection?.Parent as ProjectItem;
			}

			if (parts.Count == 0)
			{
				return string.Empty;
			}

			return string.Join("/", parts);
		}
	}
}