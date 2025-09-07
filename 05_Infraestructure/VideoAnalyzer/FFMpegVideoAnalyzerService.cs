using Application.Abstractions.VideoAnalyser;
using Domain.Entities;
using FFMpegCore;

namespace Infraestructure.VideoAnalyser;
public class FFMpegVideoAnalyzerService : IVideoFrameAnalyzerService
{
    public FFMpegVideoAnalyzerService()
    {
        ConfigureFFMpegBinaryPath();
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
    
    private void ConfigureFFMpegBinaryPath()
    {
        // Determine the OS-specific folder for FFMpeg binaries, Default is Linux.
        var osFolder = "linux-x64"; 
        
        if (OperatingSystem.IsWindows())
        {
            osFolder = "win-x64";
        }
        
        var ffmpegBinaryFolderPath = Path.Combine(AppContext.BaseDirectory, "Assets", "FFMpeg", osFolder);

        GlobalFFOptions.Configure(options => options.BinaryFolder = ffmpegBinaryFolderPath);
    }
}
