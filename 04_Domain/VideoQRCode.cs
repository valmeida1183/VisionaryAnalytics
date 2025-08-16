using SharedKernel.Abstractions;

namespace Domain;
public class VideoQRCode : Entity
{
    public TimeSpan TimeStamp { get; set; }
    public string? DataContent { get; set; }
    public Guid VideoProcessResultId { get; set; }

    public virtual VideoProcessResult? VideoProcessResult { get; set; }
}
