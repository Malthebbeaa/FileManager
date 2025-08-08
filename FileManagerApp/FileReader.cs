using Spectre.Console;
using System.IO;
namespace FileManagerApp;

public class FileReader
{
    public static void Test2(string path)
    {
        if (File.Exists(path))
        {
            string[] files = Directory.GetFiles(path);
            var chosenfile = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Directories")
                    .PageSize(10)
                    .AddChoices(files));
            
            Test2(chosenfile);
        }
        string[] dirs = Directory.GetDirectories(path);
        var chosenDir = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("Directories")
                .PageSize(10)
                .AddChoices(dirs));
        
        Test2(chosenDir);
    }

    public static void Explore(string path)
    {
        while (true)
        {
            if (File.Exists(path))
            {
                
                AnsiConsole.MarkupLine($"File {Path.GetFileName(path)} explored");
                AnsiConsole.Prompt(new TextPrompt<string>("Press b, then enter to return..."));
                return;
            }

            if (!Directory.Exists(path))
            {
                AnsiConsole.MarkupLine("Invalid Path");
                return;
            }

            var choices = CreateChoices(path);

            var chosen = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title($"[underline green]Contents of {path}[/]")
                    .PageSize(15)
                    .AddChoices(choices));

            if (chosen.Equals(".. Go back"))
            {
                var parent = Directory.GetParent(path);
                if (parent == null)
                {
                    return;
                }
                
                path = parent.FullName;
                continue;
            }
            var parts = chosen.Split('|');
            var filename = parts[1];
            var fullpath = Path.Combine(path, filename);
            
            path = fullpath;
        }
    }
    
    public static List<string> CreateChoices(string path)
    {
        var directories = Directory.GetDirectories(path);
        var files = Directory.GetFiles(path);
            
        var choices = new List<string>();
        choices.Add(".. Go back");
        choices.AddRange(directories.Select(d => $"[blue]:file_folder:[/]|{Path.GetFileName(d)}"));
        choices.AddRange(files.Select(f => $":page_facing_up:|{Path.GetFileName(f)}"));

        return choices;
    }

    public static List<FileItem> GetDirectoryContents(string path)
    {
        var items = new List<FileItem>();

        try
        {
            var directoryInfo = new DirectoryInfo(path);

            foreach (var dir in directoryInfo.GetDirectories())
            {
                items.Add(new FileItem()
                {
                    Name = dir.Name,
                    Size = "-",
                    Modified = dir.LastWriteTime.ToString("yyyy-MM-dd HH:mm"),
                    IsDirectory = true,
                    FullPath = dir.FullName
                });
            }

            foreach (var file in directoryInfo.GetFiles())
            {
                items.Add(new FileItem()
                {
                    Name = file.Name,
                    Size = FormatFileSize(file.Length),
                    Modified = file.LastWriteTime.ToString("yyyy-MM-dd HH:mm"),
                    IsDirectory = false,
                    FullPath = file.FullName
                });
            }
        }
        catch (UnauthorizedAccessException)
        {
            
        }
        catch (DirectoryNotFoundException)
        {
            
        }
        return items;
    }

    private static string FormatFileSize(long bytes)
    {
        string[] suffixes = {"B", "KB", "MB", "GB", "TB"};
        int counter = 0;
        decimal number = bytes;

        while (Math.Round(number / 1024) >= 1)
        {
            number = number / 1024;
            counter++;
        }

        return $"{number:n1} {suffixes[counter]}";
    }
}