using Application.VideoProcesses.GetById;
using MediatR;
using SharedKernel.Primitives;
using WebApi.Extensions;

namespace WebApi.Endpoints.VideoProcesses;

public static class GetStatusById
{
    public static void MapGetStatusByIdVideoProcessEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapGet("videos/status/{id:guid}", async (Guid id, ISender sender, CancellationToken cancellationToken) =>
        {
            var query = new GetVideoProcessStatusByIdQuery(id);

            Result<string> result = await sender.Send(query, cancellationToken);

            return result.Match(Results.Ok, Results.NotFound);
        })
        .Produces(StatusCodes.Status200OK)
        .WithName("VideoProcessStatus")
        .WithTags(EndpointTags.VideoProcess);
    }
}
