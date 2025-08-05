using Spectre.Console;

namespace FileManagerApp;

public class ActionHandler
{
    public static FileItem Create(string path)
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

    public static void RenameFile(FileItem fileItem)
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
}