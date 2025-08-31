using Application.VideoProcesses.Create;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SharedKernel.Primitives;
using WebApi.Extensions;

namespace WebApi.Endpoints.Video;

public static class VideoEndpoints 
{
    public static void AddVideoEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("videos", async (IFormFile file, ISender sender, CancellationToken cancellationToken) =>
        {
            var command = new CreateVideoProcessCommand(file);

            Result<VideoProcess> result = await sender.Send(command, cancellationToken);

            return result.Match(Results.Ok, Results.BadRequest);
        })
        .Accepts<IFormFile>("multipart/form-data")
        .DisableAntiforgery()
        .Produces(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status400BadRequest)
        .WithName("UploadVideo")
        .WithTags(EndpointTags.VideoProcess)
        .WithMetadata(new RequestSizeLimitAttribute(200_000_000));
    }
}
