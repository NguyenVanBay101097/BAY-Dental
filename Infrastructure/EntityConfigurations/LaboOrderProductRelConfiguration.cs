using ApplicationCore.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.EntityConfigurations
{
    public class LaboOrderProductRelConfiguration : IEntityTypeConfiguration<LaboOrderProductRel>
    {
        public void Configure(EntityTypeBuilder<LaboOrderProductRel> builder)
        {
            builder.HasKey(x => new { x.LaboOrderId, x.ProductId});

            builder.HasOne(x => x.LaboOrder)
            .WithMany(x => x.LaboOrderProductRel)
            .HasForeignKey(x => x.LaboOrderId);

            builder.HasOne(x => x.Product)
                .WithMany()
                .HasForeignKey(x => x.ProductId);
        }
    }
}
