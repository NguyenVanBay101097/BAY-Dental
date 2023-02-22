using ApplicationCore.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.EntityConfigurations
{
    public class CompanyConfiguration : IEntityTypeConfiguration<Company>
    {
        public void Configure(EntityTypeBuilder<Company> builder)
        {
            builder.Property(x => x.Name)
               .IsRequired();

            builder.HasOne(x => x.Partner)
              .WithMany()
              .HasForeignKey(x => x.PartnerId)
              .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.AccountIncome)
              .WithMany()
              .HasForeignKey(x => x.AccountIncomeId);

            builder.HasOne(x => x.AccountExpense)
               .WithMany()
               .HasForeignKey(x => x.AccountExpenseId);

            builder.HasOne(x => x.CreatedBy)
             .WithMany()
             .HasForeignKey(x => x.CreatedById);

            builder.HasOne(x => x.WriteBy)
           .WithMany()
           .HasForeignKey(x => x.WriteById);
        }
    }
}
