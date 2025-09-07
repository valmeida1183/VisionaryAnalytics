using Domain.Entities;

namespace Application.Abstractions.QrCodeAnalyzer;
public interface IQrCodeAnalyzerService
{
    Task<IEnumerable<VideoQRCode>> DecodeQrCodeFromImages(IEnumerable<string> framesPaths, VideoProcess videoProcess);
}
