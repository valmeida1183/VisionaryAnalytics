using Application.Abstractions.Messaging;
using Application.Abstractions.QrCodeAnalyzer;
using Application.Abstractions.Repositories;
using Application.Abstractions.Storage;
using Application.Abstractions.VideoAnalyser;
using Application.Extensions;
using Domain.Entities;
using FluentValidation;
using SharedKernel.Enums;
using SharedKernel.Primitives;

namespace Application.VideoProcesses.Analyze;
internal sealed class AnalyzeVideoProcessCommandHandler : ICommandHandler<AnalyzeVideoProcessCommand>
{
    private readonly IVideoProcessRepository _videoProcessRepository;
    private readonly IVideoStorageService _videoStorageService;
    private readonly IVideoFrameAnalyzerService _videoFrameAnalyserService;
    private readonly IQrCodeAnalyzerService _qrCodeAnalyzerService;
    private readonly IValidator<AnalyzeVideoProcessCommand> _validator;
    private readonly IUnitOfWork _unitOfWork;

    public AnalyzeVideoProcessCommandHandler(
        IVideoProcessRepository videoProcessRepository,
        IVideoStorageService videoStorageService,
        IVideoFrameAnalyzerService videoFrameAnalyserService,
        IQrCodeAnalyzerService qrCodeAnalyzerService,
        IValidator<AnalyzeVideoProcessCommand> validator,
        IUnitOfWork unitOfWork)
    {
        _videoProcessRepository = videoProcessRepository;
        _videoStorageService = videoStorageService;
        _videoFrameAnalyserService = videoFrameAnalyserService;
        _qrCodeAnalyzerService = qrCodeAnalyzerService;
        _validator = validator;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(AnalyzeVideoProcessCommand command, CancellationToken cancellationToken)
    {
        var validationResult = _validator.Validate(command);

        if (!validationResult.IsValid)
        {
            return Result.Failure(validationResult.ResultErrors());
        }

        return await AnalyzeVideo(command.VideoProcess!, command.VideoProcess!.FolderPath!, cancellationToken);
    }

    private async Task<Result> AnalyzeVideo(
        VideoProcess videoProcess,
        string videoFolderPath,
        CancellationToken cancellationToken)
    {
        try
        {
            await SetVideoProcessStatus(
                videoProcess, 
                ProcessStatus.InProcess, 
                cancellationToken);

            var frames = await _videoFrameAnalyserService
                .ExtractImagesFramesAsync(videoFolderPath, videoProcess!);

            //TODO call _qrCodeAnalyzerService


            await SetVideoProcessStatus(
                videoProcess, 
                ProcessStatus.Finished, 
                cancellationToken);

            return Result.Success();
        }
        catch (Exception ex)
        {
            await SetVideoProcessStatus(
                videoProcess, 
                ProcessStatus.Failure, 
                cancellationToken);

            return Result.Failure(new List<Error> { Error.Failure("VideoProcess.AnalyzeError", ex.Message) });
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
