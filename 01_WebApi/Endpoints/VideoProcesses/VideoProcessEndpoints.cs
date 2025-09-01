using Application.VideoProcesses.Create;
using Application.VideoProcesses.GetById;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SharedKernel.Primitives;
using WebApi.Extensions;

namespace WebApi.Endpoints.Video;

public static class VideoProcessEndpoints 
{
    public static void AddVideoProcessEndpoints(this IEndpointRouteBuilder app)
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
