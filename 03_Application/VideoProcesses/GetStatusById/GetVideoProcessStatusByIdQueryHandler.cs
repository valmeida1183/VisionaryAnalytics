using Application.Abstractions.Messaging;
using Application.Abstractions.Repositories;
using SharedKernel.Primitives;

namespace Application.VideoProcesses.GetById;
internal sealed class GetVideoProcessStatusByIdQueryHandler(IVideoProcessRepository videoProcessRepository) : IQueryHandler<GetVideoProcessStatusByIdQuery, string>
{
    public async Task<Result<string>> Handle(GetVideoProcessStatusByIdQuery query, CancellationToken cancellationToken)
    {
        var videoProcess =  await videoProcessRepository
            .GetByIdAsync(query.videoProcessId, cancellationToken);

        if (videoProcess is null)
        {
            return Result.Failure<string>(
                new List<Error> { 
                    Error.NotFound("NotFound", $"The video process with Id {query.videoProcessId} was not found.") 
                });
        }

        var videoStatus = videoProcess.Status.ToString();

        return Result.Success(videoStatus);
    }
}
