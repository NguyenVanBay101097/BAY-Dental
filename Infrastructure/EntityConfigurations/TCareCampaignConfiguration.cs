using ApplicationCore.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.EntityConfigurations
{
    public class TCareCampaignConfiguration : IEntityTypeConfiguration<TCareCampaign>
    {
        public void Configure(EntityTypeBuilder<TCareCampaign> builder)
        {
            builder.Property(x => x.Name).IsRequired();

            builder.HasOne(x => x.CreatedBy)
            .WithMany()
            .HasForeignKey(x => x.CreatedById);

            builder.HasOne(x => x.Tag)
                .WithMany()
                .HasForeignKey(x => x.TagId);

            builder.HasOne(x => x.FacebookPage)
              .WithMany()
              .HasForeignKey(x => x.FacebookPageId);

            builder.HasOne(x => x.WriteBy)
                .WithMany()
                .HasForeignKey(x => x.WriteById);

            builder.HasOne(x => x.TCareScenario)
                .WithMany(x => x.Campaigns)
                .HasForeignKey(x => x.TCareScenarioId);
        }
    }
}
