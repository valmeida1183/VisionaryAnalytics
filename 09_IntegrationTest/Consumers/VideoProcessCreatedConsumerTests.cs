
using Application.VideoProcesses.Analyze;
using Application.VideoProcesses.Create;
using Domain.Entities;
using Infraestructure.Database;
using MassTransit.Testing;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using SharedKernel.Enums;

namespace IntegrationTest.Consumers;

public class VideoProcessCreatedConsumerTests : IClassFixture<ConsumerTestWebApplicationFactory<Program>>
{
    private readonly ITestHarness _harness;
    private readonly IServiceProvider _services;
    private readonly Mock<ISender> _senderMock;

    public VideoProcessCreatedConsumerTests(ConsumerTestWebApplicationFactory<Program> factory)
    {
        _harness = factory.Services.GetRequiredService<ITestHarness>();
        _services = factory.Services;
        _senderMock = factory.SenderMock;
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task Consume_WhenVideoProcessExists_ShouldSendCommand()
    {
        // Arrange
        await using var scope = _services.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var videoProcess = new VideoProcess
        {
            Id = Guid.NewGuid(),
            FileName = "test.mp4",
            FileExtension = ".mp4",
            OriginalName = "test.mp4",
            Status = ProcessStatus.InProcess, // The consumer expects this status
            Size = 123
        };
        await dbContext.Set<VideoProcess>().AddAsync(videoProcess);
        await dbContext.SaveChangesAsync();

        var integrationEvent = new VideoProcessCreatedIntegrationEvent(videoProcess.Id);

        // Act
        await _harness.Bus.Publish(integrationEvent);

        // Assert
        // 1. Check if the message was consumed
        Assert.True(await _harness.Consumed
            .Any<VideoProcessCreatedIntegrationEvent>(x => x.Context.Message.Id == videoProcess.Id));

        // 2. Check if the consumer sent the command
        _senderMock.Verify(
            x => x.Send(
                It.Is<AnalyzeVideoProcessCommand>(cmd => cmd.VideoProcess.Id == videoProcess.Id),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }
}
