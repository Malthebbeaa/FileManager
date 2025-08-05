using Spectre.Console;
namespace FileManagerApp;

public class FileDisplayer
{
    public static Panel CreateFileTable(List<FileItem> files, int selectedIndex)
    {
        var table = new Table()
            .BorderColor(Color.Grey)
            .Border(TableBorder.None);

        table.AddColumn(new TableColumn("[bold]Name[/]").Width(25));
        table.AddColumn(new TableColumn("[bold]Size[/]").Width(10).RightAligned());
        table.AddColumn(new TableColumn("[bold]Modified[/]").Width(20));

        for (int i = 0; i < files.Count; i++)
        {
            var file = files[i];
            var nameColor = file.IsDirectory ? "blue" : "white";
            var icon = file.IsDirectory ? "📁" : GetFileIcon(file.Name);
            
            var nameMarkup = i == selectedIndex 
                ? $"[black on white]{icon} {file.Name}[/]"
                : $"[{nameColor}]{icon} {file.Name}[/]";

            var sizeMarkup = i == selectedIndex
                ? $"[black on white]{file.Size}[/]"
                : file.Size;

            var modifiedMarkup = i == selectedIndex
                ? $"[black on white]{file.Modified}[/]"
                : $"[dim]{file.Modified}[/]";

            table.AddRow(nameMarkup, sizeMarkup, modifiedMarkup);
        }

        if (files.Count == 0)
        {
            table.AddRow("[dim]No files found[/]", "", "");
        }

        return new Panel(table)
            .Border(BoxBorder.Rounded)
            .BorderColor(Color.Grey)
            .Padding(1, 0);
    }
    
    static string GetFileIcon(string fileName)
    {
        var extension = Path.GetExtension(fileName).ToLower();
        return extension switch
        {
            ".txt" or ".md" or ".readme" => "📄",
            ".cs" or ".js" or ".py" or ".java" or ".cpp" or ".c" => "⚡",
            ".json" or ".xml" or ".yml" or ".yaml" => "⚙️",
            ".jpg" or ".jpeg" or ".png" or ".gif" or ".bmp" => "🖼️",
            ".mp3" or ".wav" or ".flac" => "🎵",
            ".mp4" or ".avi" or ".mkv" => "🎬",
            ".pdf" => "📕",
            ".zip" or ".rar" or ".7z" => "📦",
            ".exe" or ".msi" => "⚡",
            ".dll" => "🔧",
            ".log" => "📋",
            _ => "📄"
        };
    }
    
}