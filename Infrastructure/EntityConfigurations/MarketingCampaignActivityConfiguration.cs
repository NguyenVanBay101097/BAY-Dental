using ApplicationCore.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.EntityConfigurations
{
    public class MarketingCampaignActivityConfiguration : IEntityTypeConfiguration<MarketingCampaignActivity>
    {
        public void Configure(EntityTypeBuilder<MarketingCampaignActivity> builder)
        {
            builder.Property(x => x.Name).IsRequired();

            builder.HasOne(x => x.Message)
                .WithMany()
                .HasForeignKey(x => x.MessageId);

            //builder.HasOne(x => x.Parent)
            //.WithMany()
            //.HasForeignKey(x => x.ParentId);
            


            builder.HasOne(x => x.Campaign)
            .WithMany(x => x.Activities)
            .HasForeignKey(x => x.CampaignId);

            builder.HasOne(x => x.CreatedBy)
          .WithMany()
          .HasForeignKey(x => x.CreatedById);

            builder.HasOne(x => x.WriteBy)
                .WithMany()
                .HasForeignKey(x => x.WriteById);
        }
    }
}
