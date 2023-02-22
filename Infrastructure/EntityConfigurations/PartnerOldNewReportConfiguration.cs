using ApplicationCore.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.EntityConfigurations
{
    class PartnerOldNewReportConfiguration : IEntityTypeConfiguration<PartnerOldNewReport>
    {
        public void Configure(EntityTypeBuilder<PartnerOldNewReport> builder)
        {
            builder.ToView("partner_old_new_report");
            builder.HasNoKey();

            builder.HasOne(x => x.Partner)
                .WithMany()
                .HasForeignKey(x => x.PartnerId);

            builder.HasOne(x => x.Order)
                .WithMany()
                .HasForeignKey(x => x.OrderId);

            builder.HasOne(x => x.Company)
               .WithMany()
               .HasForeignKey(x => x.CompanyId);
        }
    }
}
