using FileManagerApp;
using Spectre.Console;

var directoryPath = "/Users/malthebrahebjerregaard/Desktop";
bool exitRequested = false;
int selectedIndex = 0;
List<FileItem> files = new List<FileItem>();

while (!exitRequested)
{
    Console.Clear();
    files = FileReader.GetDirectoryContents(directoryPath);
    Panel panel = FileDisplayer.CreateFileTable(files, selectedIndex);

    AnsiConsole.Write(panel);
    AnsiConsole.WriteLine();

    ShowControls();

    // Handle keyboard input
    var key = Console.ReadKey(true);
    switch (key.Key)
    {
        case ConsoleKey.UpArrow:
        case ConsoleKey.K:
            if (selectedIndex > 0) selectedIndex--;
            break;

        case ConsoleKey.DownArrow:
        case ConsoleKey.J:
            if (selectedIndex < files.Count - 1) selectedIndex++;
            break;

        case ConsoleKey.Enter:
            if (files.Count > 0)
            {
                var selectedFile = files[selectedIndex];
                if (selectedFile.IsDirectory)
                {
                    try
                    {
                        directoryPath = selectedFile.FullPath;
                        files = FileReader.GetDirectoryContents(directoryPath);
                        selectedIndex = 0;
                    }
                    catch (UnauthorizedAccessException)
                    {
                        // Handle permission denied
                        AnsiConsole.MarkupLine("[red]Access denied to this directory[/]");
                        Console.ReadKey();
                    }
                }
            }

            break;

        case ConsoleKey.Backspace:
            var parent = Directory.GetParent(directoryPath);
            if (parent != null)
            {
                directoryPath = parent.FullName;
                files = FileReader.GetDirectoryContents(directoryPath);
                selectedIndex = 0;
            }

            break;

        case ConsoleKey.A:
            FileItem fileItem = ActionHandler.Create(directoryPath);
            if (fileItem.IsDirectory)
            {
                Directory.CreateDirectory(fileItem.FullPath);
            }
            else
            {
                File.Create(fileItem.FullPath).Close();
            }
            break;
        case ConsoleKey.D:
            FileItem fileItemToRemove = files[selectedIndex];
            ActionHandler.Delete(fileItemToRemove);
            break;
        case ConsoleKey.Q:
        case ConsoleKey.Escape:
            exitRequested = true;
            break;

        case ConsoleKey.R:
            FileItem fileItemToRename = files[selectedIndex];
            ActionHandler.RenameFile(fileItemToRename);
            break;
    }

    static void ShowControls()
    {
        AnsiConsole.WriteLine();
        AnsiConsole.MarkupLine(
            "[dim]Use [/][bold]↑/↓[/][dim] or [/][bold]K/J[/][dim] Move | [/][bold]Enter[/][dim] Open | [/][bold]R[/][dim] Rename | [/][bold]A[/][dim] Add | [/][bold]D[/][dim] Delete | [/][bold]Backspace[/][dim] Go Back | [/][bold]Q[/][dim] Quit[/]");
    }
}