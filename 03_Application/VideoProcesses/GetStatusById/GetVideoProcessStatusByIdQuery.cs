using Application.Abstractions.Messaging;
using Domain.Entities;

namespace Application.VideoProcesses.GetById;
public sealed record GetVideoProcessStatusByIdQuery(Guid videoProcessId) : IQuery<string>;

