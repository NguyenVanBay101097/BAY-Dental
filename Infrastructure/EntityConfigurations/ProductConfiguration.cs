using ApplicationCore.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.EntityConfigurations
{
    public class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.Property(x => x.Name)
               .IsRequired();

            builder.HasOne(x => x.Categ)
                .WithMany()
                .HasForeignKey(x => x.CategId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Company)
               .WithMany()
               .HasForeignKey(x => x.CompanyId);

            builder.HasOne(x => x.UOM)
              .WithMany()
              .HasForeignKey(x => x.UOMId)
              .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.UOMPO)
              .WithMany()
              .HasForeignKey(x => x.UOMPOId)
              .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.CreatedBy)
        .WithMany()
        .HasForeignKey(x => x.CreatedById);

            builder.HasOne(x => x.WriteBy)
           .WithMany()
           .HasForeignKey(x => x.WriteById);

            builder.Property(p => p.ListPrice)
                .HasColumnType("decimal(18,2)");
        }
    }
}
