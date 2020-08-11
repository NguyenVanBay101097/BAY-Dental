﻿using ApplicationCore.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.EntityConfigurations
{
    public class ChamCongConfiguration : IEntityTypeConfiguration<ChamCong>
    {
        public void Configure(EntityTypeBuilder<ChamCong> builder)
        {
            builder.HasOne(u => u.Employee)
                .WithMany(x=> x.ChamCongs)
                .HasForeignKey(u => u.EmployeeId);

            builder.HasOne(x => x.Company)
             .WithMany()
             .HasForeignKey(x => x.CompanyId);

            builder.HasOne(x => x.CreatedBy)
          .WithMany()
          .HasForeignKey(x => x.CreatedById);

            builder.HasOne(x => x.WriteBy)
                .WithMany()
                .HasForeignKey(x => x.WriteById);
            builder.HasOne(x => x.WorkEntryType)
                .WithMany(x => x.ChamCongs)
                .HasForeignKey(x => x.WorkEntryTypeId).OnDelete(DeleteBehavior.Restrict);
        }
    }
}
