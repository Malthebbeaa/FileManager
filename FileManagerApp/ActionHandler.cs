using System.Diagnostics;
using Spectre.Console;

namespace FileManagerApp;

public class ActionHandler
{
    public static FileItem CreateFileItem(string path)
    {
        Console.Clear();
        var type = AnsiConsole.Prompt(new SelectionPrompt<string>()
            .Title("File or Directory")
            .PageSize(5)
            .AddChoices(["File", "Directory"]));
        var name = AnsiConsole.Ask<string>("Name:");

        var fileItem = new FileItem()
        {
            Name = name,
            FullPath = Path.Combine(path, name),
            IsDirectory = type == "Directory",
            Modified = DateTime.Today.ToString("yyyy-MM-dd HH:mm"),
            Size = type == "File" ? "0 B" : "-"
        };
        
        return fileItem;
    }

    public static void Create(FileItem fileItem)
    {
        if (fileItem.IsDirectory)
        {
            Directory.CreateDirectory(fileItem.FullPath);
        }
        else
        {
            File.Create(fileItem.FullPath).Close();
        }
    }

    public static void Delete(FileItem fileItem)
    {
        Console.Clear();
        var confirmation = AnsiConsole.Prompt(new SelectionPrompt<string>()
            .Title("Are you sure you want to delete " + fileItem.Name + "?")
            .PageSize(3)
            .AddChoices(["Yes", "No"]));

        if (confirmation.Equals("Yes"))
        {
            if (fileItem.IsDirectory)
            {
                Directory.Delete(fileItem.FullPath, true);
            }
            else
            {
                File.Delete(fileItem.FullPath);
            }
        }
    }

    public static void Rename(FileItem fileItem)
    {
        Console.Clear();
        var path = fileItem.FullPath;
        var newFileName = AnsiConsole.Ask<string>("New Filename for " + fileItem.Name + ":");

        fileItem.Name = newFileName;
        var parent = Directory.GetParent(path);
        var newFullPath = Path.Combine(parent.FullName, newFileName);
        if (fileItem.IsDirectory)
        {
            Directory.Move(path, newFullPath);
        }
        else
        {
            File.Move(path, newFullPath);
        }
    }

    public static void OpenInEditor(FileItem fileItem)
    {
        try
        {
            var editor = ChooseEditor();

            switch (editor)
            {
                case "Rider":
                    Process.Start("open", $"-na \"/Applications/Rider.app\" --args \"{fileItem.FullPath}\"");
                    break;
                case "IntelliJ":
                    Process.Start("open", $"-na \"/Users/malthebrahebjerregaard/Applications/IntelliJ IDEA.app\" --args \"{fileItem.FullPath}\"");
                    break;
                 case "VS Code":
                    Process.Start("code",fileItem.FullPath);
                    break;
                default:
                    Process.Start("code",  fileItem.FullPath);
                    break;
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    static string ChooseEditor()
    {
        var editor = AnsiConsole.Prompt(new SelectionPrompt<string>()
            .Title("Which editor do you want to open?")
            .PageSize(5)
            .AddChoices(["VS Code", "IntelliJ", "Rider"]));
        return editor;
    }
}