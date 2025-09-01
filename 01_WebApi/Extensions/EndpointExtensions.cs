using WebApi.Endpoints.Video;
using WebApi.Endpoints.VideoQrCodes;

namespace WebApi.Extensions;

public static class EndpointExtensions
{
    public static IEndpointRouteBuilder AddEndpoints(this IEndpointRouteBuilder app)
    {
        app.AddVideoProcessEndpoints();
        app.AddVideoQrCodeEndpoints();

        return app;
    }
}
