using ApplicationCore.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.EntityConfigurations
{
    class QuotationLineConfiguration : IEntityTypeConfiguration<QuotationLine>
    {
        public void Configure(EntityTypeBuilder<QuotationLine> builder)
        {
            builder.HasOne(x => x.CreatedBy)
                .WithMany()
                .HasForeignKey(x => x.CreatedById);

            builder.HasOne(x => x.WriteBy)
                .WithMany()
                .HasForeignKey(x => x.WriteById);

            builder.HasOne(x => x.Product)
                .WithMany()
                .HasForeignKey(x => x.ProductId);

            builder.HasOne(x => x.Doctor)
                .WithMany()
                .HasForeignKey(x => x.DoctorId);

            builder.HasOne(x => x.Assistant)
                .WithMany()
                .HasForeignKey(x => x.AssistantId);

            builder.HasOne(x => x.ToothCategory)
                .WithMany()
                .HasForeignKey(x => x.ToothCategoryId);

            builder.HasOne(x => x.Quotation)
                .WithMany(x => x.Lines)
                .HasForeignKey(x => x.QuotationId);
        }
    }
}
