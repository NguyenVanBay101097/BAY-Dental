using ApplicationCore.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.EntityConfigurations
{
    public class HrPayslipLineConfiguration : IEntityTypeConfiguration<HrPayslipLine>
    {
        public void Configure(EntityTypeBuilder<HrPayslipLine> builder)
        {
            builder.Property(x => x.Name).IsRequired();

            builder.HasOne(x => x.Slip)
                .WithMany(x => x.Lines)
                .HasForeignKey(x => x.SlipId);

            builder.HasOne(x => x.Category)
              .WithMany()
              .HasForeignKey(x => x.CategoryId);

            builder.HasOne(x => x.SalaryRule)
           .WithMany()
           .HasForeignKey(x => x.SalaryRuleId)
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
