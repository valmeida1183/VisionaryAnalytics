using Application.Abstractions.Messaging;

namespace Application.VideoProcesses.GetById;
public sealed record GetVideoProcessStatusByIdQuery(Guid videoProcessId) : IQuery<string>;

