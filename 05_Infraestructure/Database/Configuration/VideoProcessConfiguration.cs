using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MongoDB.EntityFrameworkCore.Extensions;

namespace Infraestructure.Database.Configuration;
internal sealed class VideoProcessConfiguration : IEntityTypeConfiguration<VideoProcess>
{
    public void Configure(EntityTypeBuilder<VideoProcess> builder)
    {
        builder.HasKey(vpr => vpr.Id);
        builder.Property(vpr => vpr.Id).HasElementName("_id");

        builder.Property(vpr => vpr.FileName).HasElementName("fileName");
        builder.Property(vpr => vpr.FileExtension).HasElementName("fileExtension");
        builder.Property(vpr => vpr.FolderPath).HasElementName("folderPath");
        builder.Property(vpr => vpr.OriginalName).HasElementName("originalName");
        builder.Property(vpr => vpr.Size).HasElementName("size");
        builder.Property(vpr => vpr.FramePerSecond).HasElementName("framePerSecond");
        builder.Property(vpr => vpr.ProcessedOn).HasElementName("processedOn");
        builder.Property(vpr => vpr.Status).HasElementName("status");
        builder.Property(vpr => vpr.CreatedOn).HasElementName("createdOn");

        builder.ToCollection("video_process_results");
    }
}
