using FluentValidation;
using SharedKernel.Enums;

namespace Application.VideoProcesses.Analyze;
internal sealed class AnalyzeVideoProcessCommandValidator: AbstractValidator<AnalyzeVideoProcessCommand>
{
    public AnalyzeVideoProcessCommandValidator()
    {
        RuleFor(x => x.VideoProcess)
            .NotNull()
            .WithErrorCode("VideoProcess.Null")
            .WithMessage("VideoProcess cannot be null.")
            .ChildRules(vp =>
            {
                vp.RuleFor(v => v.Status)
                    .Equal(ProcessStatus.Pending)
                    .WithErrorCode("VideoProcess.InvalidStatus")
                    .WithMessage("VideoProcess status must be Pending to be analyzed.");

                vp.RuleFor(v => v.FolderPath)
                    .NotEmpty()
                    .WithErrorCode("VideoProcess.FolderPathEmpty")
                    .WithMessage("FolderPath cannot be null or empty.")
                    .Must(folderPath => Directory.Exists(folderPath))
                    .WithErrorCode("VideoProcess.FolderPathNotExist")
                    .WithMessage("FolderPath does not exist.");
            });        
    }
}
