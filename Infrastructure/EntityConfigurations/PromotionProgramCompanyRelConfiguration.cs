using ApplicationCore.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.EntityConfigurations
{
    public class PromotionProgramCompanyRelConfiguration : IEntityTypeConfiguration<PromotionProgramCompanyRel>
    {
        public void Configure(EntityTypeBuilder<PromotionProgramCompanyRel> builder)
        {
            builder.HasKey(x => new { x.PromotionProgramId, x.CompanyId });

            builder.HasOne(x => x.PromotionProgram)
              .WithMany(x => x.ProgramCompanyRels)
              .HasForeignKey(x => x.PromotionProgramId);

            builder.HasOne(x => x.Company)
             .WithMany()
             .HasForeignKey(x => x.CompanyId);
        }
    }
}
