﻿using ApplicationCore.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.EntityConfigurations
{
    public class HrPayslipConfiguration : IEntityTypeConfiguration<HrPayslip>
    {
        public void Configure(EntityTypeBuilder<HrPayslip> builder)
        {
            builder.Property(x => x.Name).IsRequired();

            builder.HasOne(x => x.Employee)
                .WithMany()
                .HasForeignKey(x => x.EmployeeId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Company)
              .WithMany()
              .HasForeignKey(x => x.CompanyId)
              .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Struct)
               .WithMany()
               .HasForeignKey(x => x.StructId);

            builder.HasOne(x => x.AccountMove)
               .WithMany()
               .HasForeignKey(x => x.AccountMoveId);

            builder.HasOne(x => x.CreatedBy)
          .WithMany()
          .HasForeignKey(x => x.CreatedById);

            builder.HasOne(x => x.WriteBy)
                .WithMany()
                .HasForeignKey(x => x.WriteById);
        }
    }
}
