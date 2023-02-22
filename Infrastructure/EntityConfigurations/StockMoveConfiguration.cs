using ApplicationCore.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.EntityConfigurations
{
    public class StockMoveConfiguration : IEntityTypeConfiguration<StockMove>
    {
        public void Configure(EntityTypeBuilder<StockMove> builder)
        {
            builder.Property(x => x.Name).IsRequired();

            builder.HasOne(x => x.Product)
                .WithMany()
                .HasForeignKey(x => x.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.LocationDest)
                .WithMany()
                .HasForeignKey(x => x.LocationDestId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Location)
                .WithMany()
                .HasForeignKey(x => x.LocationId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Picking)
                .WithMany(x => x.MoveLines)
                .HasForeignKey(x => x.PickingId);

            builder.HasOne(x => x.PickingType)
                 .WithMany()
                 .HasForeignKey(x => x.PickingTypeId);

            builder.HasOne(x => x.Partner)
                .WithMany()
                .HasForeignKey(x => x.PartnerId);

            builder.HasOne(x => x.Warehouse)
              .WithMany()
              .HasForeignKey(x => x.WarehouseId);

            builder.HasOne(x => x.PurchaseLine)
          .WithMany()
          .HasForeignKey(x => x.PurchaseLineId);

            builder.HasOne(x => x.ProductUOM)
               .WithMany()
               .HasForeignKey(x => x.ProductUOMId);

            builder.HasOne(x => x.Company)
                .WithMany()
                .HasForeignKey(x => x.CompanyId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Inventory)
             .WithMany(x => x.Moves)
             .HasForeignKey(x => x.InventoryId);

            builder.HasOne(x => x.CreatedBy)
          .WithMany()
          .HasForeignKey(x => x.CreatedById);

            builder.HasOne(x => x.WriteBy)
                .WithMany()
                .HasForeignKey(x => x.WriteById);
        }
    }
}
