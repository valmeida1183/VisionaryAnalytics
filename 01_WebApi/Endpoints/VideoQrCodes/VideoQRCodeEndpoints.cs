using Application.VideoQrCodes.GetByVideoProcessId;
using Domain.Entities;
using MediatR;
using SharedKernel.Primitives;
using WebApi.Extensions;

namespace WebApi.Endpoints.VideoQrCodes;

public static class VideoQRCodeEndpoints
{
    public static void AddVideoQrCodeEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("qrcodes/{videoProcessId:guid}", async (Guid videoProcessId, ISender sender, CancellationToken cancellationToken) =>
        {
            var query = new GetQRCodesByVideoProcessIdQuery(videoProcessId);

            Result<IEnumerable<VideoQRCode>> result = await sender.Send(query, cancellationToken);

            return result.Match(Results.Ok, Results.BadRequest);
        })
        .Produces(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound)
        .WithName("VideoQrCodes")
        .WithTags(EndpointTags.QrCodes);
    }
}
