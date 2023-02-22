using ApplicationCore.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.EntityConfigurations
{
    public class SmsMessageConfiguration : IEntityTypeConfiguration<SmsMessage>
    {
        public void Configure(EntityTypeBuilder<SmsMessage> builder)
        {
            builder.HasOne(x => x.WriteBy)
                 .WithMany()
                 .HasForeignKey(x => x.WriteById);

            builder.HasOne(x => x.CreatedBy)
                .WithMany()
                .HasForeignKey(x => x.CreatedById);

            builder.HasOne(x => x.SmsTemplate)
                .WithMany()
                .HasForeignKey(x => x.SmsTemplateId);

            builder.HasOne(x => x.SmsCampaign)
                .WithMany()
                .HasForeignKey(x => x.SmsCampaignId);

            builder.HasOne(x => x.WriteBy)
                .WithMany()
                .HasForeignKey(x => x.WriteById);
        }
    }
}
