using JobBoard.Domain.JobPost;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JobBoard.Infrastructure.Persistence.Configurations;

public class JobPostTagConfiguration : IEntityTypeConfiguration<JobPostTagEntity>
{
    public void Configure(EntityTypeBuilder<JobPostTagEntity> builder)
    {
        builder.HasKey(jpt => new { jpt.JobPostId, jpt.TagId });

        builder
            .HasOne(jpt => jpt.JobPost)
            .WithMany(jp => jp.JobPostTags)
            .HasForeignKey(jpt => jpt.JobPostId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasOne(jpt => jpt.Tag)
            .WithMany(t => t.JobPostTags)
            .HasForeignKey(jpt => jpt.TagId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}