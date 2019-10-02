using ApplicationCore.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.EntityConfigurations
{
    public class ProductCompanyRelConfiguration : IEntityTypeConfiguration<ProductCompanyRel>
    {
        public void Configure(EntityTypeBuilder<ProductCompanyRel> builder)
        {
            builder.HasKey(x => new { x.ProductId, x.CompanyId });

            builder.HasOne(x => x.Product)
                .WithMany(x => x.ProductCompanyRels)
                .HasForeignKey(x => x.ProductId);

            builder.HasOne(x => x.Company)
               .WithMany()
               .HasForeignKey(x => x.CompanyId);
        }
    }
}
