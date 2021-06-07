using ApplicationCore.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.EntityConfigurations
{
    public class SmsMessageDetailConfiguration : IEntityTypeConfiguration<SmsMessageDetail>
    {
        public void Configure(EntityTypeBuilder<SmsMessageDetail> builder)
        {
            builder.HasOne(x => x.CreatedBy)
                 .WithMany()
                 .HasForeignKey(x => x.CreatedById);

            builder.HasOne(x => x.WriteBy)
                .WithMany()
                .HasForeignKey(x => x.WriteById);

            builder.HasOne(x => x.Partner)
               .WithMany()
               .HasForeignKey(x => x.PartnerId);

            builder.HasOne(x => x.SmsMessage)
                .WithMany()
                .HasForeignKey(x => x.SmsMessageId);

            builder.HasOne(x => x.SmsCampaign)
                .WithMany()
                .HasForeignKey(x => x.SmsCampaignId);
        }
    }
}
