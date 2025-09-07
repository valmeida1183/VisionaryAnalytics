using Application.Abstractions.QrCodeAnalyzer;
using Domain.Entities;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System.Collections.Concurrent;
using System.Text.RegularExpressions;
using ZXing;
using ZXing.Common;

namespace Infraestructure.QrCodeAnalyzer;
public sealed class ZxingQrCodeAnalyzerService : IQrCodeAnalyzerService
{
    public ZxingQrCodeAnalyzerService()
    {        
    }

    public async Task<IEnumerable<VideoQRCode>> DecodeQrCodeFromImages(IEnumerable<string> framesPaths, VideoProcess videoProcess)
    {
        var qrCodes = new ConcurrentDictionary<string, VideoQRCode>();

        await Parallel.ForEachAsync(framesPaths, async (framePath, cancellationToken) =>
        {
            var qrDataContent = DecodeQrCodeFromImage(framePath);

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

    private string? DecodeQrCodeFromImage(string imagePath)
    {
        using var image = Image.Load<Rgba32>(imagePath);

        // Copy pixels to array of bytes (RGBA format)
        var pixels = new byte[image.Width * image.Height * 4];
        image.CopyPixelDataTo(pixels);

        // Create a luminance source from the pixel data
        var luminance = new RGBLuminanceSource(
            pixels,
            image.Width,
            image.Height,
            RGBLuminanceSource.BitmapFormat.RGBA32);

        // Create a binary bitmap from the luminance source
        var binarizer = new HybridBinarizer(luminance);
        var binaryBitmap = new BinaryBitmap(binarizer);

        // Configure the reader to look specifically for QR codes
        var reader = new MultiFormatReader();
        var hints = new Dictionary<DecodeHintType, object>
        {
            { DecodeHintType.POSSIBLE_FORMATS, new List<BarcodeFormat> { BarcodeFormat.QR_CODE } },
            { DecodeHintType.TRY_HARDER, true }
        };

        var result = reader.decode(binaryBitmap, hints);

        return result?.Text;
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
