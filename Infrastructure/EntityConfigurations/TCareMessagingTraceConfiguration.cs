using ApplicationCore.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.EntityConfigurations
{
    public class TCareMessagingTraceConfiguration : IEntityTypeConfiguration<TCareMessagingTrace>
    {
        public void Configure(EntityTypeBuilder<TCareMessagingTrace> builder)
        {
            builder.HasOne(x => x.TCareCampaign)
                .WithMany(x => x.Traces)
                .HasForeignKey(x => x.TCareCampaignId);

            builder.HasOne(x => x.ChannelSocial)
               .WithMany()
               .HasForeignKey(x => x.ChannelSocialId)
               .OnDelete(DeleteBehavior.SetNull);

            builder.HasOne(x => x.Partner)
            .WithMany()
            .HasForeignKey(x => x.PartnerId)
            .OnDelete(DeleteBehavior.SetNull);

            builder.HasOne(x => x.CreatedBy)
          .WithMany()
          .HasForeignKey(x => x.CreatedById);

            builder.HasOne(x => x.WriteBy)
                .WithMany()
                .HasForeignKey(x => x.WriteById);

            builder.HasOne(x => x.TCareMessaging)
                .WithMany(x => x.TCareMessagingTraces)
                .HasForeignKey(x => x.TCareMessagingId);
        }
    }
}
