namespace FileManagerApp;

public class FileItem
{
    public string Name { get; set; } = string.Empty;
    public string Size { get; set; } = string.Empty;
    public string FullPath { get; set; } = string.Empty;
    public bool IsDirectory { get; set; } = false;
    public string Modified { get; set; }  = string.Empty;
}