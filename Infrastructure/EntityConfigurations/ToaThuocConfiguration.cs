using ApplicationCore.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.EntityConfigurations
{
    public class ToaThuocConfiguration : IEntityTypeConfiguration<ToaThuoc>
    {
        public void Configure(EntityTypeBuilder<ToaThuoc> builder)
        {
            builder.Property(x => x.Name).IsRequired();

            builder.HasOne(x => x.DotKham)
                .WithMany()
                .HasForeignKey(x => x.DotKhamId);

            builder.HasOne(x => x.Partner)
               .WithMany()
               .HasForeignKey(x => x.PartnerId)
            .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.User)
               .WithMany()
               .HasForeignKey(x => x.UserId);

            builder.HasOne(x => x.Employee)
             .WithMany()
             .HasForeignKey(x => x.EmployeeId);

            builder.HasOne(x => x.SaleOrder)
                .WithMany()
                .HasForeignKey(x => x.SaleOrderID)
                .OnDelete(DeleteBehavior.Restrict);

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
