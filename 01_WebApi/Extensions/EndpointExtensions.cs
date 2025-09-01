using WebApi.Endpoints.Video;
using WebApi.Endpoints.VideoProcesses;
using WebApi.Endpoints.VideoQrCodes;

namespace WebApi.Extensions;

public static class EndpointExtensions
{
    public static IEndpointRouteBuilder AddEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapCreateVideoProcessEndpoint();
        app.MapGetStatusByIdVideoProcessEndpoint();
        app.MapGetByVideoProcessIdQRCodesEndpoint();

        return app;
    }
}
