using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ContentGrabber
{
	class MarkdownFileFormatter
	{
		static readonly IReadOnlyDictionary<string, string> LanguageMap =
			new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
			{
				[".cs"]           = "csharp",
				[".csproj"]       = "xml",
				[".props"]        = "xml",
				[".targets"]      = "xml",
				[".xaml"]         = "xml",
				[".xml"]          = "xml",
				[".json"]         = "json",
				[".yml"]          = "yaml",
				[".yaml"]         = "yaml",
				[".md"]           = "markdown",
				[".sql"]          = "sql",
				[".js"]           = "javascript",
				[".ts"]           = "typescript",
				[".tsx"]          = "tsx",
				[".jsx"]          = "jsx",
				[".html"]         = "html",
				[".htm"]          = "html",
				[".css"]          = "css",
				[".scss"]         = "scss",
				[".razor"]        = "razor",
				[".txt"]          = "text",
				[".ps1"]          = "powershell",
				[".bat"]          = "bat",
				[".cmd"]          = "bat",
				[".sh"]           = "bash",
				[".config"]       = "xml",
				[".editorconfig"] = "ini",
				[".sln"]          = "text",
			};

		public string BuildMarkdown(IReadOnlyList<SelectedFile> files)
		{
			var builder = new StringBuilder();

			foreach (SelectedFile file in files)
			{
				string content = file.CurrentContent;

				if (content == null)
				{
					if (!File.Exists(file.FullPath))
					{
						continue;
					}

					content = File.ReadAllText(file.FullPath);
				}

				string extension = Path.GetExtension(file.FullPath);
				string language = GetLanguage(extension);

				builder.AppendLine(file.DisplayPath + ":");
				builder.AppendLine("```"            + language);
				builder.AppendLine(EscapeFenceContent(content));
				builder.AppendLine("```");
				builder.AppendLine();
			}

			return builder.ToString().TrimEnd();
		}

		string GetLanguage(string extension)
		{
			if (LanguageMap.TryGetValue(extension, out string language))
			{
				return language;
			}

			return string.Empty;
		}

		string EscapeFenceContent(string content)
		{
			return content.Replace("```", "``\\`");
		}
	}
}