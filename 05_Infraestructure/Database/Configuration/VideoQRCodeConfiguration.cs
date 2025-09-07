using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MongoDB.EntityFrameworkCore.Extensions;

namespace Infraestructure.Database.Configuration;
internal sealed class VideoQRCodeConfiguration : IEntityTypeConfiguration<VideoQRCode>
{
    public void Configure(EntityTypeBuilder<VideoQRCode> builder)
    {
        builder.HasKey(vqc => vqc.Id);
        builder.Property(vqc => vqc.Id).HasElementName("_id");

        builder.Property(vqc => vqc.TimeStamp).HasElementName("timeStamp");
        builder.Property(vqc => vqc.DataContent).HasElementName("dataContent");
        builder.Property(vqc => vqc.VideoProcessId).HasElementName("videoProcessId");
        builder.Property(vqc => vqc.CreatedOn).HasElementName("createdOn");

        builder.HasIndex("VideoProcessId");

        builder.ToCollection("video_qrcodes");
    }
}
