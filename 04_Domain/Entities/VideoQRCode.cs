using SharedKernel.Abstractions;
using System.Text.Json;

namespace Domain.Entities;
public class VideoQRCode : Entity
{
    public TimeSpan TimeStamp { get; set; }
    public required string DataContent { get; set; }
    public Guid VideoProcessResultId { get; set; }

    public virtual VideoProcessResult? VideoProcessResult { get; set; }
}
