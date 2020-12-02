﻿using ApplicationCore.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.EntityConfigurations
{
    public class DotKhamLineConfiguration : IEntityTypeConfiguration<DotKhamLine>
    {
        public void Configure(EntityTypeBuilder<DotKhamLine> builder)
        {
            builder.HasOne(x => x.DotKham)
                .WithMany(x => x.Lines)
                .HasForeignKey(x => x.DotKhamId);

            builder.HasOne(x => x.Product)
               .WithMany()
               .HasForeignKey(x => x.ProductId);

            builder.HasOne(x => x.SaleOrderLine)
             .WithMany()
             .HasForeignKey(x => x.SaleOrderLineId);

            builder.HasOne(x => x.CreatedBy)
            .WithMany()
            .HasForeignKey(x => x.CreatedById);

            builder.HasOne(x => x.WriteBy)
                .WithMany()
                .HasForeignKey(x => x.WriteById);
        }
    }
}
