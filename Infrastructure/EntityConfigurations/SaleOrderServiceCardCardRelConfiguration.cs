using ApplicationCore.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.EntityConfigurations
{
    public class SaleOrderServiceCardCardRelConfiguration : IEntityTypeConfiguration<SaleOrderServiceCardCardRel>
    {
        public void Configure(EntityTypeBuilder<SaleOrderServiceCardCardRel> builder)
        {
            builder.HasOne(x => x.SaleOrder)
             .WithMany(x => x.SaleOrderCardRels)
             .HasForeignKey(x => x.SaleOrderId);

            builder.HasOne(x => x.Card)
           .WithMany(x => x.SaleOrderCardRels)
           .HasForeignKey(x => x.CardId);

            builder.HasOne(x => x.CreatedBy)
            .WithMany()
            .HasForeignKey(x => x.CreatedById);

            builder.HasOne(x => x.WriteBy)
                .WithMany()
                .HasForeignKey(x => x.WriteById);
        }
    }
}
