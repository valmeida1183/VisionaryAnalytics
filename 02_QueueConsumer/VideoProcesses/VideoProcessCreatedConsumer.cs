using Application.Abstractions.Repositories;
using Application.Abstractions.Storage;
using Application.VideoProcesses.Create;
using MassTransit;
using SharedKernel.Enums;

namespace QueueConsumer.VideoProcesses;
internal sealed class VideoProcessCreatedConsumer : IConsumer<VideoProcessCreatedIntegrationEvent>
{
    private readonly IVideoProcessRepository _videoProcessRepository;
    private readonly IVideoStorageService _videoStorageService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<VideoProcessCreatedIntegrationEvent> _logger;

    public VideoProcessCreatedConsumer(
        IVideoProcessRepository videoProcessRepository,
        IVideoStorageService videoStorageService,
        IUnitOfWork unitOfWork,
        ILogger<VideoProcessCreatedIntegrationEvent> logger)
    {
        _videoProcessRepository = videoProcessRepository;
        _videoStorageService = videoStorageService;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<VideoProcessCreatedIntegrationEvent> context)
    {
        var message = context.Message;

        _logger.LogInformation("Received VideoProcessCreatedIntegrationEvent for VideoProcess Id: {VideoProcessId}", message.Id);
        
        var videoProcess = await _videoProcessRepository.GetByIdAsync(message.Id);
        
        if (videoProcess is null)
        {
            _logger.LogError("VideoProcess with Id: {VideoProcessId} not found.", message.Id);
            return;
        }

        if (videoProcess.Status != ProcessStatus.Pending)
        {
            _logger.LogError("VideoProcess with Id: {VideoProcessId} is not in Pending status. Current status: {Status}", message.Id, videoProcess.Status);
            return;
        }

        try
        {
            // Simulate video processing
            _logger.LogInformation("Processing video for VideoProcess Id: {VideoProcessId}", message.Id);
            await Task.Delay(TimeSpan.FromSeconds(10)); // Simulate processing time
            // Update status to Completed
            videoProcess.Status = ProcessStatus.Finished;
            
            _videoProcessRepository.Update(videoProcess);
            await _unitOfWork.SaveChangesAsync(context.CancellationToken);
            _logger.LogInformation("Video processing completed for VideoProcess Id: {VideoProcessId}", message.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing video for VideoProcess Id: {VideoProcessId}", message.Id);
            videoProcess.Status = ProcessStatus.Failure;

            _videoProcessRepository.Update(videoProcess);
            await _unitOfWork.SaveChangesAsync(context.CancellationToken);
        }
    }
}
