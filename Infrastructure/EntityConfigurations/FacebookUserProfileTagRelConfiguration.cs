using ApplicationCore.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.EntityConfigurations
{
    public class FacebookUserProfileTagRelConfiguration : IEntityTypeConfiguration<FacebookUserProfileTagRel>
    {
        public void Configure(EntityTypeBuilder<FacebookUserProfileTagRel> builder)
        {
            builder.HasKey(x => new { x.UserProfileId, x.TagId });

            builder.HasOne(x => x.UserProfile)
              .WithMany(x => x.TagRels)
              .HasForeignKey(x => x.UserProfileId);

            builder.HasOne(x => x.Tag)
             .WithMany()
             .HasForeignKey(x => x.TagId);
        }
    }
}
