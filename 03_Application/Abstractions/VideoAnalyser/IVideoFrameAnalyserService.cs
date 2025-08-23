using Domain.Entities;
using SharedKernel.Primitives;

namespace Application.Abstractions.VideoAnalyser;
public interface IVideoFrameAnalyserService
{
    Task<IEnumerable<string>> ExtractFramesAsync(string videoFolderPath, VideoProcess videoProcess, int frameRate = 1);
    Result ValidateVideoAnalysisProcess(string videoFolderPath, VideoProcess? videoProcess);
}
