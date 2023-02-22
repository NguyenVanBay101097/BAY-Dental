using ApplicationCore.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.EntityConfigurations
{
    public class StockQuantConfiguration : IEntityTypeConfiguration<StockQuant>
    {
        public void Configure(EntityTypeBuilder<StockQuant> builder)
        {
            builder.HasOne(x => x.Product)
                .WithMany(x => x.StockQuants)
                .HasForeignKey(x => x.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Location)
                .WithMany()
                .HasForeignKey(x => x.LocationId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.NegativeMove)
              .WithMany()
              .HasForeignKey(x => x.NegativeMoveId);

            builder.HasOne(x => x.PropagatedFrom)
              .WithMany()
              .HasForeignKey(x => x.PropagatedFromId);

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
