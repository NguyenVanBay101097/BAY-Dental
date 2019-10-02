using ApplicationCore.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.EntityConfigurations
{
    public class StockPickingConfiguration : IEntityTypeConfiguration<StockPicking>
    {
        public void Configure(EntityTypeBuilder<StockPicking> builder)
        {
            builder.HasOne(u => u.PickingType)
                .WithMany()
                .HasForeignKey(u => u.PickingTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(u => u.Location)
                .WithMany()
                .HasForeignKey(u => u.LocationId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(u => u.LocationDest)
                .WithMany()
                .HasForeignKey(u => u.LocationDestId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(u => u.Partner)
                .WithMany()
                .HasForeignKey(u => u.PartnerId);

            builder.HasOne(x => x.Company)
             .WithMany()
             .HasForeignKey(x => x.CompanyId)
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
