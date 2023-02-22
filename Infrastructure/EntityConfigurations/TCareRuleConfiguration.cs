using ApplicationCore.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.EntityConfigurations
{
    public class TCareRuleConfiguration : IEntityTypeConfiguration<TCareRule>
    {
        public void Configure(EntityTypeBuilder<TCareRule> builder)
        {
            builder.Property(x => x.Type).IsRequired();

            builder.HasOne(x => x.Campaign)
              .WithMany()
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
