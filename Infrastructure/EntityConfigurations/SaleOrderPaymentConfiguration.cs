using ApplicationCore.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.EntityConfigurations
{
    public class SaleOrderPaymentConfiguration : IEntityTypeConfiguration<SaleOrderPayment>
    {
        public void Configure(EntityTypeBuilder<SaleOrderPayment> builder)
        {

            builder.HasOne(x => x.Move)
                .WithMany()
                .HasForeignKey(x => x.MoveId);

            builder.HasOne(x => x.PaymentMove)
                .WithMany()
                .HasForeignKey(x => x.PaymentMoveId);

            builder.HasOne(x => x.Order)
             .WithMany(x => x.SaleOrderPayments)
             .HasForeignKey(x => x.OrderId);

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
