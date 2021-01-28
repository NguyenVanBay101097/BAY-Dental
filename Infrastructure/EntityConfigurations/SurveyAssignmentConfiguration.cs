﻿using ApplicationCore.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.EntityConfigurations
{
    class SurveyAssignmentConfiguration : IEntityTypeConfiguration<SurveyAssignment>
    {
        public void Configure(EntityTypeBuilder<SurveyAssignment> builder)
        {

            builder.HasOne(x => x.employee)
           .WithMany()
           .HasForeignKey(x => x.EmployeeId);

            builder.HasOne(x => x.SaleOrder)
           .WithMany()
           .HasForeignKey(x => x.SaleOrderId);

            builder.HasOne(x => x.Company)
            .WithMany()
            .HasForeignKey(x => x.CompanyId);

            builder.HasOne(x => x.CreatedBy)
            .WithMany()
            .HasForeignKey(x => x.CreatedById);

            builder.HasOne(x => x.WriteBy)
                .WithMany()
                .HasForeignKey(x => x.WriteById);
        }
    }
}
