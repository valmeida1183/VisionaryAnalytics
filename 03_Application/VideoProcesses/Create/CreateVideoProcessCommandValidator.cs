using FluentValidation;

namespace Application.VideoProcesses.Create;
internal sealed class CreateVideoProcessCommandValidator : AbstractValidator<CreateVideoProcessCommand>
{
    public CreateVideoProcessCommandValidator()
    {
        const long Mb = 1024 * 1024;
        const long maxFileSize = 200 * Mb; // 200 MB
        var allowedExtensions = new[] { ".mp4", ".avi" };

        RuleFor(x => x.File)
            .NotNull()
            .WithErrorCode("Video.FileRequired")
            .WithMessage("File is required.")

            .Must(file => file.Length > 0)
            .WithErrorCode("Video.Empty")
            .WithMessage("File cannot be empty.")

            .Must(file => file.Length <= maxFileSize)
            .WithErrorCode("Video.FileSizeExceeded")
            .WithMessage($"File size exceeds the limit of {maxFileSize / Mb} MB.")

            .Must(file => file.ContentType == "video/mp4" || file.ContentType == "video/x-msvideo" || file.ContentType == "video/avi")
            .WithErrorCode("Video.InvalidFileType")
            .WithMessage("Only MP4 and AVI video formats are supported.")

            .Must(file => allowedExtensions.Contains(Path.GetExtension(file.FileName).ToLowerInvariant()))
            .WithErrorCode("Video.InvalidFileExtension")
            .WithMessage("Invalid file extension. Only .mp4 or .avi allowed.");
    }
}
