using Application.Abstractions.Messaging;
using Application.Abstractions.Repositories;
using Domain.Entities;
using SharedKernel.Primitives;

namespace Application.VideoQrCodes.GetByVideoProcessId;
public sealed class GetQRCodesByVideoProcessIdQueryHandler(IVideoQrCodeRepository videoQrCodeRepository) 
    : IQueryHandler<GetQRCodesByVideoProcessIdQuery, IEnumerable<VideoQRCode>>
{
    public async Task<Result<IEnumerable<VideoQRCode>>> Handle(GetQRCodesByVideoProcessIdQuery query, CancellationToken cancellationToken)
    {
        var qrCodes =  await videoQrCodeRepository.GetByVideoProcessIdAsync(query.VideoProcessId, cancellationToken);

        return Result.Success(qrCodes);
    }
}
