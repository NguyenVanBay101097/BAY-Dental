using ApplicationCore.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.EntityConfigurations
{
    public class MailTrackingValueConfiguration : IEntityTypeConfiguration<MailTrackingValue>
    {
        public void Configure(EntityTypeBuilder<MailTrackingValue> builder)
        {
            builder.Property(x => x.Field).IsRequired();
            builder.Property(x => x.FieldDesc).IsRequired();

            builder.HasOne(x => x.MailMessage)
              .WithMany(x => x.TrackingValues)
              .HasForeignKey(x => x.MailMessageId);

            builder.HasOne(x => x.CreatedBy)
             .WithMany()
             .HasForeignKey(x => x.CreatedById);

            builder.HasOne(x => x.WriteBy)
           .WithMany()
           .HasForeignKey(x => x.WriteById);
        }
    }
}
