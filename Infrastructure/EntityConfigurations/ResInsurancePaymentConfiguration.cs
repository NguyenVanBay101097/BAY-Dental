using ApplicationCore.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.EntityConfigurations
{
    public class ResInsurancePaymentConfiguration : IEntityTypeConfiguration<ResInsurancePayment>
    {
        public void Configure(EntityTypeBuilder<ResInsurancePayment> builder)
        {

            builder.HasOne(x => x.Order)
             .WithMany()
             .HasForeignKey(x => x.OrderId);

            builder.HasOne(x => x.SaleOrderPayment)
            .WithMany()
            .HasForeignKey(x => x.SaleOrderPaymentId);

            builder.HasOne(x => x.Company)
                .WithMany()
                .HasForeignKey(x => x.CompanyId);


            builder.HasOne(x => x.CreatedBy)
                    .WithMany()
                    .HasForeignKey(x => x.CreatedById);

            builder.HasOne(x => x.WriteBy)
                .WithMany()
                .HasForeignKey(x => x.WriteById);
        }
    }
}
