using Application.Abstractions.Repositories;
using Application.VideoProcesses.Analyze;
using Application.VideoProcesses.Create;
using MassTransit;
using MediatR;

namespace QueueConsumer.VideoProcesses;
public sealed class VideoProcessCreatedConsumer : IConsumer<VideoProcessCreatedIntegrationEvent>
{
    private readonly ISender _sender;
    private readonly IVideoProcessRepository _videoProcessRepository;
    private readonly ILogger<VideoProcessCreatedIntegrationEvent> _logger;

    public VideoProcessCreatedConsumer(
        ISender sender,
        IVideoProcessRepository videoProcessRepository,
        ILogger<VideoProcessCreatedIntegrationEvent> logger)
    {
        _sender = sender;
        _videoProcessRepository = videoProcessRepository;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<VideoProcessCreatedIntegrationEvent> context)
    {
        var message = context.Message;
        _logger.LogInformation("Received VideoProcessCreatedIntegrationEvent for VideoProcess Id: {VideoProcessId}", message.Id);
        
        var videoProcess = await _videoProcessRepository.GetByIdAsync(message.Id, context.CancellationToken);

        if (videoProcess is null)
            return;

        var command = new AnalyzeVideoProcessCommand(videoProcess);
        var result = await _sender.Send(command, context.CancellationToken);

        if (result.IsFailure)
        {
            foreach (var error in result.Errors)
            {
                _logger.LogError("Error: {ErrorCode} - {ErrorMessage}", error.Code, error.Description);
            }

            return;
        }

        _logger.LogInformation("The video was processed successfully!");        
    }   
}
