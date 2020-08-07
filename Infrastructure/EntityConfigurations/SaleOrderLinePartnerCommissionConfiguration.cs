﻿using ApplicationCore.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.EntityConfigurations
{
    public class SaleOrderLinePartnerCommissionConfiguration : IEntityTypeConfiguration<SaleOrderLinePartnerCommission>
    {
        public void Configure(EntityTypeBuilder<SaleOrderLinePartnerCommission> builder)
        {
            builder.HasOne(x => x.Partner)
               .WithMany()
               .HasForeignKey(x => x.PartnerId)
               .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.SaleOrderLine)
              .WithMany()
              .HasForeignKey(x => x.SaleOrderLine);

            builder.HasOne(x => x.Commission)
              .WithMany()
              .HasForeignKey(x => x.CommissionId)
              .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.CreatedBy)
              .WithMany()
              .HasForeignKey(x => x.CreatedById);

            builder.HasOne(x => x.WriteBy)
              .WithMany()
              .HasForeignKey(x => x.WriteById);
        }
    }
}
