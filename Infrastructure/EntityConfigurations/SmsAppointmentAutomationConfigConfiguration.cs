using ApplicationCore.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.EntityConfigurations
{
    public class SmsAppointmentAutomationConfigConfiguration : IEntityTypeConfiguration<SmsAppointmentAutomationConfig>
    {
        public void Configure(EntityTypeBuilder<SmsAppointmentAutomationConfig> builder)
        {
            builder.HasOne(x => x.CreatedBy)
                .WithMany()
                .HasForeignKey(x => x.CreatedById);

            builder.HasOne(x => x.WriteBy)
                .WithMany()
                .HasForeignKey(x => x.WriteById);

            builder.HasOne(x => x.Company)
                .WithMany()
                .HasForeignKey(x => x.CompanyId);

            builder.HasOne(x => x.Template)
              .WithMany()
              .HasForeignKey(x => x.TemplateId);

            builder.HasOne(x => x.SmsAccount)
              .WithMany()
              .HasForeignKey(x => x.SmsAccountId);

            builder.HasOne(x => x.SmsCampaign)
           .WithMany()
           .HasForeignKey(x => x.SmsCampaignId);
        }
    }
}
