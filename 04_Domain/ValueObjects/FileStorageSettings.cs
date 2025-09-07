namespace Domain.ValueObjects;
public class FileStorageSettings
{
    public string Root { get; set; } = string.Empty;
    public string AppFolderName { get; set; } = string.Empty; 
    public string VideoFolderName { get; set; } = string.Empty;
}
