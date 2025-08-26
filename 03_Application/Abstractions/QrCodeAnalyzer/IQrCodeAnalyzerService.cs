namespace Application.Abstractions.QrCodeAnalyzer;
public interface IQrCodeAnalyzerService
{
    string? DecodeQrCodeFromImage(string imagePath);
}
