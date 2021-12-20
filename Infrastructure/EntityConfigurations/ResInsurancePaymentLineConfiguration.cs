using ApplicationCore.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.EntityConfigurations
{
    public class ResInsurancePaymentLineConfiguration : IEntityTypeConfiguration<ResInsurancePaymentLine>
    {
        public void Configure(EntityTypeBuilder<ResInsurancePaymentLine> builder)
        {
            builder.HasOne(x => x.SaleOrderLine)
                  .WithMany(x => x.InsurancePaymentLines)
                  .HasForeignKey(x => x.SaleOrderLineId);

            builder.HasOne(x => x.ResInsurancePayment)
                    .WithMany(x => x.Lines)
                    .HasForeignKey(x => x.ResInsurancePaymentId)
                    .OnDelete(DeleteBehavior.Restrict);


            builder.HasOne(x => x.CreatedBy)
                    .WithMany()
                    .HasForeignKey(x => x.CreatedById);

            builder.HasOne(x => x.WriteBy)
                .WithMany()
                .HasForeignKey(x => x.WriteById);
        }
    }
}
