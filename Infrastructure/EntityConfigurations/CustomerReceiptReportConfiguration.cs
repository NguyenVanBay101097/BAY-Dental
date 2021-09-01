using ApplicationCore.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.EntityConfigurations
{
    public class CustomerReceiptReportConfiguration : IEntityTypeConfiguration<CustomerReceiptReport>
    {
        public void Configure(EntityTypeBuilder<CustomerReceiptReport> builder)
        {
            builder.ToView("customer_receipt_report");

            builder.HasOne(x => x.Company)
            .WithMany()
            .HasForeignKey(x => x.CompanyId);

            builder.HasOne(x => x.Doctor)
            .WithMany()
            .HasForeignKey(x => x.DoctorId);

            builder.HasOne(x => x.Partner)
           .WithMany()
           .HasForeignKey(x => x.PartnerId);

            builder.HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserId);
        }
    }
}
