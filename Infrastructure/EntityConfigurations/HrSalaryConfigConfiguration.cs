using ApplicationCore.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.EntityConfigurations
{
    public class HrSalaryConfiguration : IEntityTypeConfiguration<HrSalaryConfig>
    {
        public void Configure(EntityTypeBuilder<HrSalaryConfig> builder)
        {
            builder.HasOne(x => x.DefaultGlobalLeaveType)
                .WithMany()
                .HasForeignKey(x => x.DefaultGlobalLeaveTypeId);

            builder.HasOne(x => x.Company)
            .WithMany()
            .HasForeignKey(x => x.CompanyId)
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
