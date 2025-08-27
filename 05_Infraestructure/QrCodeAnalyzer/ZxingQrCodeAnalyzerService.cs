using Application.Abstractions.QrCodeAnalyzer;
using Domain.Entities;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System.Collections.Concurrent;
using System.Text.RegularExpressions;
using ZXing;
using ZXing.Common;
using ZXing.ImageSharp;

namespace Infraestructure.QrCodeAnalyzer;
public sealed class ZxingQrCodeAnalyzerService : IQrCodeAnalyzerService
{
    private readonly ZXing.BarcodeReader<Image<L8>> _readerGray;

    public ZxingQrCodeAnalyzerService()
    {

        _readerGray = new ZXing.BarcodeReader<Image<L8>>(
            img => new ImageSharpLuminanceSource<L8>(img))
        {
            AutoRotate = true,
            Options = new DecodingOptions
            {
                TryHarder = true,
                PossibleFormats = [BarcodeFormat.QR_CODE]
            }
        };
    }

    public string? DecodeQrCodeFromImage(string imagePath)
    {
        using var image = Image.Load<L8>(imagePath);
        var result = _readerGray.Decode(image);

        return result?.Text;
    }

    public async Task<IEnumerable<VideoQRCode>> DecodeQrCodeFromImages(IEnumerable<string> framesPath, VideoProcess videoProcess)
    {
        var qrCodes = new ConcurrentDictionary<string, VideoQRCode>();

        await Parallel.ForEachAsync(framesPath, async (framePath, cancellationToken) =>
        {
            var qrDataContent = DecodeQrCodeFromImage(framePath);

            // TODO add a try catch to log errors

            if (!string.IsNullOrWhiteSpace(qrDataContent))
            {
                var videoQrCode = new VideoQRCode
                {
                    VideoProcessId = videoProcess.Id,
                    DataContent = qrDataContent,
                    TimeStamp = ExtractTimestampFromFrame(framePath, videoProcess.FramePerSecond)
                };

                qrCodes.TryAdd(qrDataContent, videoQrCode);
            }

            await Task.CompletedTask;
        });

        return qrCodes.Values;
    }

    private TimeSpan ExtractTimestampFromFrame(string framePath, int fps)
    {
        // Example: frame_000123.png -> 123 frames when fps = 1
        var match = Regex.Match(Path.GetFileNameWithoutExtension(framePath), @"frame_(\d+)");
        if (match.Success && int.TryParse(match.Groups[1].Value, out int frameNumber))
        {
            double seconds = frameNumber / fps;

            return TimeSpan.FromMilliseconds(Math.Round(seconds * 1000));
        }

        return TimeSpan.Zero;
    }
}
