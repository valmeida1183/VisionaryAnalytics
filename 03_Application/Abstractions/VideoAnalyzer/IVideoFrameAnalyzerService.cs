using Domain.Entities;

namespace Application.Abstractions.VideoAnalyser;
public interface IVideoFrameAnalyzerService
{
    Task<IEnumerable<string>> ExtractImagesFramesAsync(string videoFolderPath, VideoProcess videoProcess);
}
