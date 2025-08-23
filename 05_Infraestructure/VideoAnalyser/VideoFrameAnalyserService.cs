using Application.Abstractions.VideoAnalyser;
using Domain.Entities;
using FFMpegCore;
using SharedKernel.Enums;
using SharedKernel.Primitives;

namespace Infraestructure.VideoAnalyser;
public class VideoFrameAnalyserService : IVideoFrameAnalyserService
{
    private readonly string _ffmpegPath;

    public VideoFrameAnalyserService(string ffmpegPath)
    {
        _ffmpegPath = ffmpegPath;

        GlobalFFOptions.Configure(options => options.BinaryFolder = _ffmpegPath);
    }

    public async Task<IEnumerable<string>> ExtractFramesAsync(string videoFolderPath, VideoProcess videoProcess, int frameRate = 1)
    {
        var frameFiles = new List<string>();

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
                .WithCustomArgument($"-vf fps={frameRate}")
             )
            .ProcessAsynchronously();

        var frameFileNames = Directory.GetFiles(frameFolder, "frame_*.png");
        frameFiles.AddRange(frameFileNames);

        return frameFiles;
    }

    public Result ValidateVideoAnalysisProcess(string videoFolderPath, VideoProcess? videoProcess)
    {
        var errors = new List<Error>();

        if (videoProcess is null)
        {
            errors.Add(Error.Failure("VideoProcess.NotFound", "VideoProcess is null."));
        }

        if (videoProcess!.Status != ProcessStatus.Pending)
        {
            errors.Add(Error.Failure("VideoProcess.InvalidStatus", "VideoProcess status must be Pending."));
        }

        if (!Directory.Exists(videoFolderPath))
        {
            errors.Add(Error.Failure("VideoFolder.NotFound", $"Video folder not found at path: {videoFolderPath}"));
        }

        if (errors.Count != 0)
        {
            return Result.Failure(errors);
        }

        return Result.Success();
    }
}
