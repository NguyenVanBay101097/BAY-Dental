using ApplicationCore.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.EntityConfigurations
{
    public class ToothDiagnosisProductRelConfiguration : IEntityTypeConfiguration<ToothDiagnosisProductRel>
    {
        public void Configure(EntityTypeBuilder<ToothDiagnosisProductRel> builder)
        {
            builder.HasKey(x => new { x.ToothDiagnosisId, x.ProductId });

            builder.HasOne(x => x.ToothDiagnosis)
                .WithMany(x => x.ToothDiagnosisProductRels)
                .HasForeignKey(x => x.ToothDiagnosisId);

            builder.HasOne(x => x.Product)
                .WithMany(x => x.ToothDiagnosisProductRels)
                .HasForeignKey(x => x.ProductId);
        }
    }
}
