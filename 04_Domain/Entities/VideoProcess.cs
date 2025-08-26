using SharedKernel.Abstractions;
using SharedKernel.Enums;

namespace Domain.Entities;
public class VideoProcess : Entity
{
    public required string FileName { get; set; }
    public required string FileExtension { get; set; }
    public string? FolderPath { get; set; }
    public required string OriginalName { get; set; }
    public long Size { get; set; }
    public DateTime? ProcessedOn { get; set; }
    public ProcessStatus Status { get; set; }

    public virtual ICollection<VideoQRCode> QRCodes { get; set; } = [];    
}
