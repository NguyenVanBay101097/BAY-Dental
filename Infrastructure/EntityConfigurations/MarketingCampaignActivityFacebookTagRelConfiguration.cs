using ApplicationCore.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.EntityConfigurations
{
    public class MarketingCampaignActivityFacebookTagRelConfiguration : IEntityTypeConfiguration<MarketingCampaignActivityFacebookTagRel>
    {
        public void Configure(EntityTypeBuilder<MarketingCampaignActivityFacebookTagRel> builder)
        {
            builder.HasKey(x => new { x.ActivityId, x.TagId });

            builder.HasOne(x => x.Activity)
              .WithMany(x => x.TagRels)
              .HasForeignKey(x => x.ActivityId);

            builder.HasOne(x => x.Tag)
             .WithMany()
             .HasForeignKey(x => x.TagId);
        }
    }
}
