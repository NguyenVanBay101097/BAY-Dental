using ApplicationCore.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.EntityConfigurations
{
    public class SaleProductionConfiguration : IEntityTypeConfiguration<SaleProduction>
    {
        public void Configure(EntityTypeBuilder<SaleProduction> builder)
        {
            builder.HasOne(x => x.Product)
            .WithMany()
            .HasForeignKey(x => x.ProductId);

            builder.HasOne(x => x.Company)
            .WithMany()
            .HasForeignKey(x => x.CompanyId);

            builder.HasOne(x => x.CreatedBy)
          .WithMany()
          .HasForeignKey(x => x.CreatedById);

            builder.HasOne(x => x.WriteBy)
            .WithMany()
            .HasForeignKey(x => x.WriteById);
        }
    }
}
