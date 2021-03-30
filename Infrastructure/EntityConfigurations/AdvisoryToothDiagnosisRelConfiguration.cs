using ApplicationCore.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.EntityConfigurations
{
    public class AdvisoryToothDiagnosisRelConfiguration : IEntityTypeConfiguration<AdvisoryToothDiagnosisRel>
    {
        public void Configure(EntityTypeBuilder<AdvisoryToothDiagnosisRel> builder)
        {
            builder.HasKey(x => new { x.AdvisoryId, x.ToothDiagnosisId });

            builder.HasOne(x => x.Advisory)
                .WithMany(x => x.AdvisoryToothDiagnosisRels)
                .HasForeignKey(x => x.AdvisoryId);

            builder.HasOne(x => x.ToothDiagnosis)
                .WithMany(x => x.AdvisoryToothDiagnosisRels)
                .HasForeignKey(x => x.ToothDiagnosisId);
        }
    }
}
