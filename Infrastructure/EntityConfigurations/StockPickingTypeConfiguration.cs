using ApplicationCore.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.EntityConfigurations
{
    public class StockPickingTypeConfiguration : IEntityTypeConfiguration<StockPickingType>
    {
        public void Configure(EntityTypeBuilder<StockPickingType> builder)
        {
            builder.Property(x => x.Name).IsRequired();
            builder.Property(x => x.Code).IsRequired();

            builder.HasOne(x => x.IRSequence)
                .WithMany()
                .HasForeignKey(x => x.IRSequenceId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.DefaultLocationDest)
                 .WithMany()
                 .HasForeignKey(x => x.DefaultLocationDestId);

            builder.HasOne(x => x.DefaultLocationSrc)
                .WithMany()
                .HasForeignKey(x => x.DefaultLocationSrcId);

            builder.HasOne(x => x.Warehouse)
               .WithMany()
               .HasForeignKey(x => x.WarehouseId);

            builder.HasOne(x => x.ReturnPickingType)
                 .WithMany()
                 .HasForeignKey(x => x.ReturnPickingTypeId);

            builder.HasOne(x => x.CreatedBy)
          .WithMany()
          .HasForeignKey(x => x.CreatedById);

            builder.HasOne(x => x.WriteBy)
                .WithMany()
                .HasForeignKey(x => x.WriteById);
        }
    }
}
