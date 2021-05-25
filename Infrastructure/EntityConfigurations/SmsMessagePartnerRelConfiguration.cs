using ApplicationCore.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.EntityConfigurations
{
    public class SmsMessagePartnerRelConfiguration : IEntityTypeConfiguration<SmsMessagePartnerRel>
    {
        public void Configure(EntityTypeBuilder<SmsMessagePartnerRel> builder)
        {
            builder.HasKey(x => new { x.PartnerId, x.SmsMessageId });

            builder.HasOne(x => x.SmsMessage)
               .WithMany(x => x.SmsMessagePartnerRels)
               .HasForeignKey(x => x.SmsMessageId);

            builder.HasOne(x => x.Partner)
                .WithMany()
                .HasForeignKey(x => x.PartnerId);
        }
    }
}
