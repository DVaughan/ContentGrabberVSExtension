using System.Collections.Generic;
using EnvDTE;
using Microsoft.VisualStudio.Shell;

namespace ContentGrabber
{
	class ProjectPathResolver
	{
		public string TryGetProjectRelativePath(ProjectItem projectItem)
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
