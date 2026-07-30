using EnvDTE;

using Microsoft.VisualStudio.Shell;

namespace ContentGrabber
{
	class OpenDocumentContentReader
	{
		public string TryGetContent(Document document)
		{
			ThreadHelper.ThrowIfNotOnUIThread();

			if (document == null)
			{
				return null;
			}

			try
			{
				var textDocument
					= document.Object("TextDocument") as TextDocument;

				if (textDocument == null)
				{
					return null;
				}

				EditPoint startPoint
					= textDocument.StartPoint.CreateEditPoint();

				return startPoint.GetText(textDocument.EndPoint);
			}
			catch
			{
				/*
				 * Some document tabs represent designers or other
				 * document types without an ordinary text buffer.
				 */
				return null;
			}
		}
	}
}