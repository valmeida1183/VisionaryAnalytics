using Domain.Entities;

namespace Application.Abstractions.QrCodeAnalyzer;
public interface IQrCodeAnalyzerService
{
    string? DecodeQrCodeFromImage(string imagePath);
    Task<IEnumerable<VideoQRCode>> DecodeQrCodeFromImages(IEnumerable<string> framesPath, VideoProcess videoProcess);
}
