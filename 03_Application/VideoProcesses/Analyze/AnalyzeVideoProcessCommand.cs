using Application.Abstractions.Messaging;
using Domain.Entities;

namespace Application.VideoProcesses.Analyze;
public record AnalyzeVideoProcessCommand(VideoProcess VideoProcess) : ICommand;

