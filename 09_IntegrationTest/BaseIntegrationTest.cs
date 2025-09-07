
using MassTransit.Testing;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;

namespace IntegrationTest;

public abstract class BaseIntegrationTest : IClassFixture<CustomWebApplicationFactory<Program>>
{
    protected readonly HttpClient _client;
    protected readonly ITestHarness _harness;

    protected readonly IServiceProvider _services;

    protected BaseIntegrationTest(CustomWebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
        _harness = factory.Services.GetRequiredService<ITestHarness>();
        _services = factory.Services;
    }

    protected async Task<JsonElement> ParseResponse(HttpResponseMessage responseMessage)
    {   
        var jsonResponse = await responseMessage.Content.ReadAsStringAsync();
        var jsonDoc = JsonDocument.Parse(jsonResponse);
        
        return jsonDoc.RootElement;
    }
}
