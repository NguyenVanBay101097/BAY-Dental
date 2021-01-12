using ApplicationCore.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.EntityConfigurations
{
    public class ToaThuocLineConfiguration : IEntityTypeConfiguration<ToaThuocLine>
    {
        public void Configure(EntityTypeBuilder<ToaThuocLine> builder)
        {
            builder.HasOne(x => x.Product)
                .WithMany()
                .HasForeignKey(x => x.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.ToaThuoc)
              .WithMany(x => x.Lines)
              .HasForeignKey(x => x.ToaThuocId);

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
