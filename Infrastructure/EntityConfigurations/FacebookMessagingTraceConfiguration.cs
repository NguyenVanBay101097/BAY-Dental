using ApplicationCore.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.EntityConfigurations
{
    public class FacebookMessagingTraceConfiguration : IEntityTypeConfiguration<FacebookMessagingTrace>
    {
        public void Configure(EntityTypeBuilder<FacebookMessagingTrace> builder)
        {
            builder.HasOne(x => x.MassMessaging)
                .WithMany()
                .HasForeignKey(x => x.MassMessagingId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.CreatedBy)
          .WithMany()
          .HasForeignKey(x => x.CreatedById);

            builder.HasOne(x => x.WriteBy)
                .WithMany()
                .HasForeignKey(x => x.WriteById);
        }
    }
}
