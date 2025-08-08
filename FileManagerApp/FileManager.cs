using Spectre.Console;

namespace FileManagerApp;

public class FileManager
{
    private bool exitRequested;
    private int selectedIndex;
    private List<FileItem> files;
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

            //AnsiConsole.Write(previewPanel);

            var key = Console.ReadKey(true);
            HandleKeyEvent(key);
        }
        
    }
    static void ShowControls()
    {
        AnsiConsole.WriteLine();
        AnsiConsole.MarkupLine(
            "[dim]Use [/][bold]↑/↓[/][dim] Move | [/][bold]Enter[/][dim] Enter File/Dir | [/][bold]Space[/][dim] Open in Editor |[/][bold]R[/][dim] Rename | [/][bold]A[/][dim] Add | [/][bold]D[/][dim] Delete | [/][bold]Backspace[/][dim] Go Back | [/][bold]Q[/][dim] Quit[/]");
    }

    void HandleKeyEvent(ConsoleKeyInfo key)
    {
        switch (key.Key)
            {
                case ConsoleKey.UpArrow:
                    if (selectedIndex > 0) selectedIndex--;
                    break;
                
                case ConsoleKey.Spacebar:
                    var selectedFileItem = files[selectedIndex];
                    ActionHandler.OpenInEditor(selectedFileItem);
                    break;

                case ConsoleKey.DownArrow:
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
                    FileItem fileItem = ActionHandler.CreateFileItem(directoryPath);
                    ActionHandler.Create(fileItem);
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
                    ActionHandler.Rename(fileItemToRename);
                    break;
            }
    }
}