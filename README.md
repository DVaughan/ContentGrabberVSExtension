# Content Grabber

I got sick of copying and pasting code from Visual Studio into my LLM prompts, so I made this.

**Copy selected files from Solution Explorer as Markdown, ready for sharing or LLM prompts.**

Content Grabber is a lightweight Visual Studio extension that lets you quickly extract the contents of one or more files and format them as clean, structured Markdown with code fences.

Perfect for sharing code snippets, documenting examples, or pasting directly into AI tools.

## ✨ Features

* **Multi-file selection**
  Select one or more files in Solution Explorer and copy them all at once.

* **Markdown formatting**
  Each file is output with:

  * file name (or project-relative path)
  * fenced code block
  * appropriate language hint (e.g. `csharp`, `json`, `xml`)

* **Project-relative paths**
  Files are labelled using their logical location within the project for clarity.

* **Clean clipboard output**
  Instantly ready to paste into:

  * ChatGPT / other LLMs
  * documentation
  * GitHub issues
  * emails

## 📋 Example Output

````markdown
Services/UserService.cs:
```csharp
public class UserService
{
    public void DoSomething()
    {
        // ...
    }
}
```

appsettings.json:
```json
{
  "Logging": {
    "LogLevel": "Information"
  }
}
```
````

## 🚀 How to Use

1. Select one or more files in **Solution Explorer**
2. Right-click
3. Click **Copy as Markdown**
4. Paste anywhere

## 💡 Use Cases

* Sharing code with AI assistants
* Creating documentation snippets
* Sending code in messages or emails
* Reviewing multiple files quickly

## 🔒 Privacy & Safety

Content Grabber operates entirely locally:

* No network calls
* No data collection
* No external dependencies

Your code stays on your machine.

## 📄 License

MIT License

