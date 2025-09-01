using Application.Abstractions.Messaging;
using Domain.Entities;

namespace Application.VideoQrCodes.GetByVideoProcessId;
public sealed record GetQRCodesByVideoProcessIdQuery(Guid VideoProcessId) : IQuery<IEnumerable<VideoQRCode>>;

