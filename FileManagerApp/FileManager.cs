using Spectre.Console;

namespace FileManagerApp;

public class FileManager
{
    private bool exitRequested = false;
    private int selectedIndex = 0;
    private List<FileItem> files = new List<FileItem>();
    private string directoryPath;
    public void Start(string path)
    {
        exitRequested = false;
        selectedIndex = 0;
        files = new List<FileItem>();
        directoryPath = path;

        while (!exitRequested)
        {
            Console.Clear();
            files = FileReader.GetDirectoryContents(directoryPath);
            Panel panel = FileDisplayer.CreateFileTable(files, selectedIndex);
            Panel currentPathPanel = FileDisplayer.ShowCurrentPath(directoryPath);
            Panel previewPanel = FileDisplayer.CreatePreview();
            
            AnsiConsole.Write(currentPathPanel);
            AnsiConsole.Write(panel);
            ShowControls();

            AnsiConsole.Write(previewPanel);
            // Handle keyboard input
            var key = Console.ReadKey(true);
            HandleKeyEvent(key);
        }
        
    }
    static void ShowControls()
    {
        AnsiConsole.WriteLine();
        AnsiConsole.MarkupLine(
            "[dim]Use [/][bold]↑/↓[/][dim] or [/][bold]K/J[/][dim] Move | [/][bold]Enter[/][dim] Open | [/][bold]R[/][dim] Rename | [/][bold]A[/][dim] Add | [/][bold]D[/][dim] Delete | [/][bold]Backspace[/][dim] Go Back | [/][bold]Q[/][dim] Quit[/]");
    }

    void HandleKeyEvent(ConsoleKeyInfo key)
    {
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
                                selectedIndex = 0;
                            }
                            catch (UnauthorizedAccessException)
                            {
                                // Handle permission denied
                                AnsiConsole.MarkupLine("[red]Access denied to this directory[/]");
                                Console.ReadKey();
                            }
                        }
                        else
                        {
                            ActionHandler.OpenFile(selectedFile);
                        }
                    }


                    break;

                case ConsoleKey.Backspace:
                    var parent = Directory.GetParent(directoryPath);
                    if (parent != null)
                    {
                        directoryPath = parent.FullName;
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
    }
}