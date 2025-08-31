using Application.Abstractions.VideoAnalyser;
using Domain.Entities;
using Domain.ValueObjects;
using FFMpegCore;
using Microsoft.Extensions.Options;

namespace Infraestructure.VideoAnalyser;
public class FFMpegVideoAnalyzerService : IVideoFrameAnalyzerService
{
    private readonly FileStorageSettings _settings;

    public FFMpegVideoAnalyzerService(IOptions<FileStorageSettings> options)
    {
        _settings = options.Value;

        var ffmpegBinaryFolderPath = Path.Combine(
            _settings.Root,
            _settings.FFMpegFolderName,
            _settings.FFMpegBinariesFolderName);

        GlobalFFOptions.Configure(options => options.BinaryFolder = ffmpegBinaryFolderPath);
    }

    public async Task<IEnumerable<string>> ExtractImagesFramesAsync(string videoFolderPath, VideoProcess videoProcess)
    {
        var frameFilePaths = new List<string>();
        var frameFolder = Path.Combine(videoFolderPath, "Frames");
        var videoFilePath = Path.Combine(videoFolderPath, videoProcess.FileName);
        var outputPattern = Path.Combine(frameFolder, "frame_%06d.png");

        if (!Directory.Exists(frameFolder))
        {
            Directory.CreateDirectory(frameFolder);
        }

        await FFMpegArguments
            .FromFileInput(videoFilePath)
            .OutputToFile(outputPattern, overwrite: true, options => options
                .WithVideoCodec("png")
                .ForceFormat("image2")
                .WithCustomArgument($"-vf fps={videoProcess.FramePerSecond}")
             )
            .ProcessAsynchronously();

        var frameFilePathsArray = Directory.GetFiles(frameFolder, "frame_*.png");
        frameFilePaths.AddRange(frameFilePathsArray);

        return frameFilePaths;
    }    
}
