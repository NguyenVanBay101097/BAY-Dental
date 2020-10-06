using ApplicationCore.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.EntityConfigurations
{
    public class TCareMessagingConfiguration : IEntityTypeConfiguration<TCareMessaging>
    {
        public void Configure(EntityTypeBuilder<TCareMessaging> builder)
        {
          

            builder.HasOne(x => x.TCareCampaign)
           .WithMany()
           .HasForeignKey(x => x.TCareCampaignId);

            builder.HasOne(x => x.CreatedBy)
            .WithMany()
            .HasForeignKey(x => x.CreatedById);

            builder.HasOne(x => x.WriteBy)
                .WithMany()
                .HasForeignKey(x => x.WriteById);
        }
    }
}
