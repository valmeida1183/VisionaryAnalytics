using Application.Abstractions.Messaging;
using Domain.Entities;
using Microsoft.AspNetCore.Http;
using SharedKernel.Enums;

namespace Application.VideoProcesses.Create;
public sealed record CreateVideoProcessCommand(IFormFile File) : ICommand<VideoProcessResult>
{
    public static implicit operator VideoProcessResult(CreateVideoProcessCommand command)
    {
        var fileExtension = command.File.FileName.ToLowerInvariant().Split('.').Last();
        var id = Guid.NewGuid();

        return new VideoProcessResult
        {
            Id = id,
            FileName = $"{id}.{fileExtension}",
            OriginalName = command.File.FileName,
            FileExtension = fileExtension,
            Size = command.File.Length,
            ProcessedOn = null,
            Status = ProcessStatus.Pending
        };
    }
}

