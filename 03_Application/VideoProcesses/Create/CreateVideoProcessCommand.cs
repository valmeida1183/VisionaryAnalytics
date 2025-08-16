using Application.Abstractions.Messaging;
using Microsoft.AspNetCore.Http;

namespace Application.VideoProcesses.Create;
public sealed record CreateVideoProcessCommand(IFormFile File) : ICommand<Guid>;
