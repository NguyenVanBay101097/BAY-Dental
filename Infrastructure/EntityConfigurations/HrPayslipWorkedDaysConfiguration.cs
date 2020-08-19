using ApplicationCore.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.EntityConfigurations
{
    public class HrPayslipWorkedDaysConfiguration : IEntityTypeConfiguration<HrPayslipWorkedDays>
    {
        public void Configure(EntityTypeBuilder<HrPayslipWorkedDays> builder)
        {
            builder.Property(x => x.Name).IsRequired();

            builder.HasOne(x => x.Payslip)
                .WithMany(x => x.WorkedDaysLines)
                .HasForeignKey(x => x.PayslipId);

            builder.HasOne(x => x.CreatedBy)
          .WithMany()
          .HasForeignKey(x => x.CreatedById);

            builder.HasOne(x => x.WriteBy)
                .WithMany()
                .HasForeignKey(x => x.WriteById);
        }
    }
}
