using ApplicationCore.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.EntityConfigurations
{
    public class MedicineOrderLineConfiguration : IEntityTypeConfiguration<MedicineOrderLine>
    {
        public void Configure(EntityTypeBuilder<MedicineOrderLine> builder)
        {
            builder.HasOne(x => x.MedicineOrder)
            .WithMany(x=>x.MedicineOrderLines)
            .HasForeignKey(x => x.MedicineOrderId);

            builder.HasOne(x => x.ToaThuocLine)
             .WithMany()
             .HasForeignKey(x => x.ToaThuocLineId)
             .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Product)
           .WithMany()
           .HasForeignKey(x => x.ProductId);

            builder.HasOne(x => x.ProductUoM)
            .WithMany()
            .HasForeignKey(x => x.ProductUoMId);

            builder.HasOne(x => x.CreatedBy)
                  .WithMany()
                  .HasForeignKey(x => x.CreatedById);

            builder.HasOne(x => x.WriteBy)
                    .WithMany()
                    .HasForeignKey(x => x.WriteById);
        }
    }
}
