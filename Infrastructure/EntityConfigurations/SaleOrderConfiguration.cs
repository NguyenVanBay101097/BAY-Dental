using ApplicationCore.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.EntityConfigurations
{
    public class SaleOrderConfiguration : IEntityTypeConfiguration<SaleOrder>
    {
        public void Configure(EntityTypeBuilder<SaleOrder> builder)
        {
            builder.Property(x => x.Name).IsRequired();

            builder.HasOne(x => x.Partner)
                .WithMany(x => x.SaleOrders)
                .HasForeignKey(x => x.PartnerId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Pricelist)
               .WithMany()
               .HasForeignKey(x => x.PricelistId);

            builder.HasOne(x => x.Company)
                .WithMany()
                .HasForeignKey(x => x.CompanyId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Quote)
              .WithMany()
              .HasForeignKey(x => x.QuoteId);

            builder.HasOne(x => x.Order)
             .WithMany()
             .HasForeignKey(x => x.OrderId);

            builder.HasOne(x => x.User)
                .WithMany()
                .HasForeignKey(x => x.UserId);

            builder.HasOne(x => x.CodePromoProgram)
            .WithMany()
            .HasForeignKey(x => x.CodePromoProgramId)
            .OnDelete(DeleteBehavior.SetNull);

            builder.HasOne(x => x.Card)
               .WithMany()
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
