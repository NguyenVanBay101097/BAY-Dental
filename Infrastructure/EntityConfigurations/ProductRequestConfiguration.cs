using ApplicationCore.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.EntityConfigurations
{
    public class ProductRequestConfiguration : IEntityTypeConfiguration<ProductRequest>
    {
        public void Configure(EntityTypeBuilder<ProductRequest> builder)
        {

            builder.HasOne(x => x.User)
             .WithMany()
             .HasForeignKey(x => x.UserId);

            builder.HasOne(x => x.SaleOrder)
              .WithMany()
              .HasForeignKey(x => x.SaleOrderId);

            builder.HasOne(x => x.Company)
              .WithMany()
              .HasForeignKey(x => x.CompanyId);


            builder.HasOne(x => x.Picking)
              .WithMany()
              .HasForeignKey(x => x.PickingId);


            builder.HasOne(x => x.Employee)
              .WithMany()
              .HasForeignKey(x => x.EmployeeId);


            builder.HasOne(x => x.CreatedBy)
            .WithMany()
            .HasForeignKey(x => x.CreatedById);

            builder.HasOne(x => x.WriteBy)
                .WithMany()
                .HasForeignKey(x => x.WriteById);
        }
    }
}
