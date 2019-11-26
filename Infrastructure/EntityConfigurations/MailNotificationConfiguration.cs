using ApplicationCore.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.EntityConfigurations
{
    public class MailNotificationConfiguration : IEntityTypeConfiguration<MailNotification>
    {
        public void Configure(EntityTypeBuilder<MailNotification> builder)
        {
            builder.HasOne(x => x.MailMessage)
                .WithMany(x => x.Notifications)
                .HasForeignKey(x => x.MailMessageId);

            builder.HasOne(x => x.ResPartner)
                .WithMany()
                .HasForeignKey(x => x.ResPartnerId);

            builder.HasOne(x => x.CreatedBy)
          .WithMany()
          .HasForeignKey(x => x.CreatedById);

            builder.HasOne(x => x.WriteBy)
                .WithMany()
                .HasForeignKey(x => x.WriteById);
        }
    }
}
