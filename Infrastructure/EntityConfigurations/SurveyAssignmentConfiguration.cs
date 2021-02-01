using ApplicationCore.Entities;
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

            builder.HasOne(x => x.Partner)
        .WithMany()
        .HasForeignKey(x => x.PartnerId);

            builder.HasOne(x => x.UserInput)
        .WithMany()
        .HasForeignKey(x => x.UserInputId);

            builder.HasOne(x => x.Employee)
           .WithMany()
           .HasForeignKey(x => x.EmployeeId);

            builder.HasOne(x => x.SaleOrder)
           .WithMany(x=> x.Assignments)
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
