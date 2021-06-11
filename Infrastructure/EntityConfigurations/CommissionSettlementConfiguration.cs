using ApplicationCore.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.EntityConfigurations
{
    public class CommissionSettlementConfiguration : IEntityTypeConfiguration<CommissionSettlement>
    {

        public void Configure(EntityTypeBuilder<CommissionSettlement> builder)
        {
            builder.HasOne(x => x.Partner)
          .WithMany()
          .HasForeignKey(x => x.PartnerId);

            builder.HasOne(x => x.Employee)
            .WithMany()
            .HasForeignKey(x => x.EmployeeId);

            builder.HasOne(x => x.MoveLine)
            .WithMany(x => x.CommissionSettlements)
            .HasForeignKey(x => x.MoveLineId);

            builder.HasOne(x => x.Commission)
          .WithMany()
          .HasForeignKey(x => x.CommissionId);

            builder.HasOne(x => x.Product)
            .WithMany()
            .HasForeignKey(x => x.ProductId);


            builder.HasOne(x => x.CreatedBy)
            .WithMany()
            .HasForeignKey(x => x.CreatedById);

            builder.HasOne(x => x.WriteBy)
            .WithMany()
            .HasForeignKey(x => x.WriteById);

            builder.HasOne(x => x.SaleOrderLine)
                .WithMany(x => x.CommissionSettlements)
                .HasForeignKey(x => x.SaleOrderLineId);
        }
    }
}
