using ApplicationCore.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.EntityConfigurations
{
    public class MailMessageResPartnerRelConfiguration : IEntityTypeConfiguration<MailMessageResPartnerRel>
    {
        public void Configure(EntityTypeBuilder<MailMessageResPartnerRel> builder)
        {
            builder.HasKey(x => new { x.MailMessageId, x.PartnerId });
            builder.HasOne(x => x.MailMessage)
             .WithMany(x => x.Recipients)
             .HasForeignKey(x => x.MailMessageId);

            builder.HasOne(x => x.Partner)
              .WithMany()
              .HasForeignKey(x => x.PartnerId);
        }
    }
}
