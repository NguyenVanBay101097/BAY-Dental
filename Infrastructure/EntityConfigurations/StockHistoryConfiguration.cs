using ApplicationCore.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.EntityConfigurations
{
    public class StockHistoryConfiguration : IEntityTypeConfiguration<StockHistory>
    {
        public void Configure(EntityTypeBuilder<StockHistory> builder)
        {
            builder.ToView("stock_history");
            builder.HasNoKey();

            builder.HasOne(x => x.Product)
              .WithMany()
              .HasForeignKey(x => x.product_id);

            builder.HasOne(x => x.ProductCateg)
             .WithMany()
             .HasForeignKey(x => x.product_categ_id);

            builder.HasOne(x => x.Company)
             .WithMany()
             .HasForeignKey(x => x.company_id);

            builder.HasOne(x => x.Move)
            .WithMany()
            .HasForeignKey(x => x.move_id)
            .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Location)
           .WithMany()
           .HasForeignKey(x => x.location_id);
        }
    }
}
