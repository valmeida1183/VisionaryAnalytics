using SharedKernel.Abstractions;
using SharedKernel.Enums;

namespace Domain;
public class VideoProcessResult : Entity
{
    public required string FileName { get; set; }
    public required string OriginalName { get; set; }
    public required string FileExtension { get; set; }
    public long Size { get; set; }
    public DateTime ProcessedOn { get; set; }
    public ProcessStatus Status { get; set; }
    public string? ErrorMessage { get; set; }

    public virtual IEnumerable<VideoQRCode> QRCodes { get; set; } = [];
}
