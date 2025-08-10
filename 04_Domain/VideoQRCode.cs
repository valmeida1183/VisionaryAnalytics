using SharedKernel.Abstractions;

namespace Domain;
public class VideoQRCode : Entity
{
    public TimeSpan TimeStamp { get; private set; }
    public string DataContent { get; private set; }

    public VideoQRCode(TimeSpan timeStamp, string dataContent) : base()
    {
        TimeStamp = timeStamp;
        DataContent = dataContent;
    }
}
