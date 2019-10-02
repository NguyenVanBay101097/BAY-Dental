using ApplicationCore.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.EntityConfigurations
{
    public class StockWarehouseConfiguration : IEntityTypeConfiguration<StockWarehouse>
    {
        public void Configure(EntityTypeBuilder<StockWarehouse> builder)
        {
            builder.Property(x => x.Name).IsRequired();

            builder.HasOne(x => x.Location)
                .WithMany()
                .HasForeignKey(x => x.LocationId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.ViewLocation)
                 .WithMany()
                 .HasForeignKey(x => x.ViewLocationId)
                 .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.InType)
                .WithMany()
                .HasForeignKey(x => x.InTypeId);

            builder.HasOne(x => x.OutType)
                .WithMany()
                .HasForeignKey(x => x.OutTypeId);

            builder.HasOne(x => x.Company)
                 .WithMany()
                 .HasForeignKey(x => x.CompanyId)
                 .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Partner)
            .WithMany()
            .HasForeignKey(x => x.PartnerId);

            builder.HasOne(x => x.CreatedBy)
          .WithMany()
          .HasForeignKey(x => x.CreatedById);

            builder.HasOne(x => x.WriteBy)
                .WithMany()
                .HasForeignKey(x => x.WriteById);
        }
    }
}
