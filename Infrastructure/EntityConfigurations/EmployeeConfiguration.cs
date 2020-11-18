using ApplicationCore.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.EntityConfigurations
{
    public class EmployeeConfiguration : IEntityTypeConfiguration<Employee>
    {
        public void Configure(EntityTypeBuilder<Employee> builder)
        {
            builder.Property(x => x.Name).IsRequired();

            builder.HasOne(x => x.Category)
                .WithMany()
                .HasForeignKey(x => x.CategoryId);

            builder.HasOne(x => x.Partner)
              .WithMany()
              .HasForeignKey(x => x.PartnerId);

            builder.HasOne(x => x.Commission)
                 .WithMany()
                 .HasForeignKey(x => x.CommissionId);

            builder.HasOne(x => x.User)
                 .WithMany()
                 .HasForeignKey(x => x.UserId);

            builder.HasOne(x => x.StructureType)
                  .WithMany()
                  .HasForeignKey(x => x.StructureTypeId);

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
