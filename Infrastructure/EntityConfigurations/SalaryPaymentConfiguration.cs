using ApplicationCore.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.EntityConfigurations
{
    public class SalaryPaymentConfiguration : IEntityTypeConfiguration<SalaryPayment>
    {
        public void Configure(EntityTypeBuilder<SalaryPayment> builder)
        {
            builder.HasOne(x => x.Journal)
            .WithMany()
            .HasForeignKey(x => x.JournalId);

            builder.HasOne(x => x.Employee)
           .WithMany()
           .HasForeignKey(x => x.EmployeeId);

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
