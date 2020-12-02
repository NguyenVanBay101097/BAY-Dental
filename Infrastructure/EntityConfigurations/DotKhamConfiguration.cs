using ApplicationCore.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.EntityConfigurations
{
    public class DotKhamConfiguration : IEntityTypeConfiguration<DotKham>
    {
        public void Configure(EntityTypeBuilder<DotKham> builder)
        {
            builder.HasOne(x => x.SaleOrder)
             .WithMany(x => x.DotKhams)
             .HasForeignKey(x => x.SaleOrderId);

            builder.HasOne(x => x.Partner)
               .WithMany(x => x.DotKhams)
               .HasForeignKey(x => x.PartnerId);

            builder.HasOne(x => x.Company)
            .WithMany()
            .HasForeignKey(x => x.CompanyId)
            .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Appointment)
             .WithMany()
             .HasForeignKey(x => x.AppointmentId);

            builder.HasOne(x => x.CreatedBy)
          .WithMany()
          .HasForeignKey(x => x.CreatedById);

            builder.HasOne(x => x.WriteBy)
                .WithMany()
                .HasForeignKey(x => x.WriteById);

            builder.HasOne(x => x.Doctor)
                .WithMany()
                .HasForeignKey(x => x.DoctorId);
        }
    }
}
