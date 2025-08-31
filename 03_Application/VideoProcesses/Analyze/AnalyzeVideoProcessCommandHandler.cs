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
    private readonly IVideoQrCodeRepository _videoQrCodeRepository;
    private readonly IVideoStorageService _videoStorageService;
    private readonly IVideoFrameAnalyzerService _videoFrameAnalyserService;
    private readonly IQrCodeAnalyzerService _qrCodeAnalyzerService;
    private readonly IValidator<AnalyzeVideoProcessCommand> _validator;
    private readonly IUnitOfWork _unitOfWork;

    public AnalyzeVideoProcessCommandHandler(
        IVideoProcessRepository videoProcessRepository,
        IVideoQrCodeRepository videoQrCodeRepository,
        IVideoStorageService videoStorageService,
        IVideoFrameAnalyzerService videoFrameAnalyserService,
        IQrCodeAnalyzerService qrCodeAnalyzerService,
        IValidator<AnalyzeVideoProcessCommand> validator,
        IUnitOfWork unitOfWork)
    {
        _videoProcessRepository = videoProcessRepository;
        _videoQrCodeRepository = videoQrCodeRepository;
        _videoStorageService = videoStorageService;
        _videoFrameAnalyserService = videoFrameAnalyserService;
        _qrCodeAnalyzerService = qrCodeAnalyzerService;
        _validator = validator;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(AnalyzeVideoProcessCommand command, CancellationToken cancellationToken)
    {
        try
        {
            var validationResult = _validator.Validate(command);

            if (!validationResult.IsValid)
            {
                return Result.Failure(validationResult.ResultErrors());
            }

            await SetVideoProcessStatus(command.VideoProcess!,
                                        ProcessStatus.InProcess,
                                        cancellationToken);

            var result = await AnalyzeVideo(command.VideoProcess!, 
                                            command.VideoProcess!.FolderPath!, 
                                            cancellationToken);

            await SetVideoProcessStatus(command.VideoProcess!,
                                        ProcessStatus.Finished,
                                        cancellationToken);

            _videoStorageService.DeleteVideoFolder(command.VideoProcess.Id);

            return result;
        }
        catch (Exception ex)
        {
            await SetVideoProcessStatus(command.VideoProcess!,
                                        ProcessStatus.Failure,
                                        cancellationToken);

            return Result.Failure(new List<Error> { Error.Failure("VideoProcess.AnalyzeError", ex.Message) });
        }
    }

    private async Task<Result> AnalyzeVideo(
        VideoProcess videoProcess,
        string videoFolderPath,
        CancellationToken cancellationToken)
    {
        var frames = await _videoFrameAnalyserService
               .ExtractImagesFramesAsync(videoFolderPath, videoProcess!);

        var qrCodes = await _qrCodeAnalyzerService
            .DecodeQrCodeFromImages(frames, videoProcess!);

        await SaveQrCodes(qrCodes, cancellationToken);

        return Result.Success(qrCodes);
    }

    private async Task SetVideoProcessStatus(VideoProcess? videoProcess, ProcessStatus processStatus, CancellationToken cancellationToken)
    {
        if (videoProcess is null)
        {
            return;
        }

        if (processStatus == ProcessStatus.Finished)
        {
            videoProcess.ProcessedOn = DateTime.UtcNow;
        }

        videoProcess.Status = processStatus;

        _videoProcessRepository.Update(videoProcess);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    private async Task SaveQrCodes(IEnumerable<VideoQRCode> qrCodes, CancellationToken cancellationToken)
    {
        if (qrCodes is null || !qrCodes.Any())
        {
            return;
        }

        await _videoQrCodeRepository.AddRangeAsync(qrCodes);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
