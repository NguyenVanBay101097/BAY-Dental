using ApplicationCore.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.EntityConfigurations
{
    public class SaleReportConfiguration : IEntityTypeConfiguration<SaleReport>
    {
        public void Configure(EntityTypeBuilder<SaleReport> builder)
        {
            builder.ToView("sale_report");
            builder.HasNoKey();

            builder.HasOne(x => x.Product)
                .WithMany()
                .HasForeignKey(x => x.ProductId);

            builder.HasOne(x => x.ProductUOM)
               .WithMany()
               .HasForeignKey(x => x.ProductUOMId);

            builder.HasOne(x => x.Partner)
               .WithMany()
               .HasForeignKey(x => x.PartnerId);

            builder.HasOne(x => x.User)
                .WithMany()
                .HasForeignKey(x => x.UserId);

            builder.HasOne(x => x.Company)
              .WithMany()
              .HasForeignKey(x => x.CompanyId);

            builder.HasOne(x => x.Categ)
          .WithMany()
          .HasForeignKey(x => x.CategId);
        }
    }
}
