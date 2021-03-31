using ApplicationCore.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.EntityConfigurations
{
    class PaymentQuotationConfiguration : IEntityTypeConfiguration<PaymentQuotation>
    {
        public void Configure(EntityTypeBuilder<PaymentQuotation> builder)
        {
            builder.HasOne(x => x.Quotation)
                .WithMany(x => x.Payments)
                .HasForeignKey(x => x.QuotationId);

            builder.HasOne(x => x.WriteBy)
                .WithMany()
                .HasForeignKey(x => x.WriteById);

            builder.HasOne(x => x.CreatedBy)
                .WithMany()
                .HasForeignKey(x => x.CreatedById);
        }
    }
}
