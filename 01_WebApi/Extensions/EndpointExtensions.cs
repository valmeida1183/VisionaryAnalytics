using WebApi.Endpoints.Video;

namespace WebApi.Extensions;

public static class EndpointExtensions
{
    public static IEndpointRouteBuilder AddEndpoints(this IEndpointRouteBuilder app)
    {
        app.AddVideoEndpoints();

        return app;
    }
}
