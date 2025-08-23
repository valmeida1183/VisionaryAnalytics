using Application.Abstractions.Repositories;
using Application.Abstractions.Storage;
using Application.Abstractions.VideoAnalyser;
using Application.VideoProcesses.Create;
using Domain.Entities;
using MassTransit;
using SharedKernel.Enums;

namespace QueueConsumer.VideoProcesses;
public sealed class VideoProcessCreatedConsumer : IConsumer<VideoProcessCreatedIntegrationEvent>
{
    private readonly IVideoProcessRepository _videoProcessRepository;
    private readonly IVideoStorageService _videoStorageService;
    private readonly IVideoFrameAnalyserService _videoFrameAnalyserService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<VideoProcessCreatedIntegrationEvent> _logger;

    public VideoProcessCreatedConsumer(
        IVideoProcessRepository videoProcessRepository,
        IVideoStorageService videoStorageService,
        IVideoFrameAnalyserService videoFrameAnalyserService,
        IUnitOfWork unitOfWork,
        ILogger<VideoProcessCreatedIntegrationEvent> logger)
    {
        _videoProcessRepository = videoProcessRepository;
        _videoStorageService = videoStorageService;
        _videoFrameAnalyserService = videoFrameAnalyserService;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<VideoProcessCreatedIntegrationEvent> context)
    {
        var message = context.Message;

        _logger.LogInformation("Received VideoProcessCreatedIntegrationEvent for VideoProcess Id: {VideoProcessId}", message.Id);

        var videoFolderPath = _videoStorageService.GetVideoFolderPath(message.Id);
        var videoProcess = await _videoProcessRepository.GetByIdAsync(message.Id);
        var result = _videoFrameAnalyserService.ValidateVideoAnalysisProcess(videoFolderPath, videoProcess);

        if (result.IsFailure)
        {
            foreach (var error in result.Errors)
            {
                _logger.LogError("Error processing video for VideoProcess Id: {VideoProcessId}. Error: {ErrorCode} - {ErrorMessage}", message.Id, error.Code, error.Description);
            }

            await SetVideoProcessStatus(videoProcess, ProcessStatus.Failure, context.CancellationToken);
            return;
        }        

        try
        {
            _logger.LogInformation("Processing video for VideoProcess Id: {VideoProcessId}", message.Id);
            await SetVideoProcessStatus(videoProcess, ProcessStatus.InProcess, context.CancellationToken);    

            var frames = await _videoFrameAnalyserService.ExtractFramesAsync(videoFolderPath, videoProcess!);

            await SetVideoProcessStatus(videoProcess, ProcessStatus.Finished, context.CancellationToken);
            _logger.LogInformation("Video processing completed for VideoProcess Id: {VideoProcessId}", message.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing video for VideoProcess Id: {VideoProcessId}", message.Id);
            await SetVideoProcessStatus(videoProcess, ProcessStatus.Failure, context.CancellationToken);
        }
    } 

    private async Task SetVideoProcessStatus(VideoProcess? videoProcess, ProcessStatus processStatus, CancellationToken cancellationToken)
    {
        if (videoProcess is null)
        {
            return;
        }

        videoProcess.Status = processStatus;

        _videoProcessRepository.Update(videoProcess);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
