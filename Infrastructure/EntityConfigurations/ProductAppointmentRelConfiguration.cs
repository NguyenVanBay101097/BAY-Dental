using ApplicationCore.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.EntityConfigurations
{
    public class ProductAppointmentRelConfiguration : IEntityTypeConfiguration<ProductAppointmentRel>
    {
        public void Configure(EntityTypeBuilder<ProductAppointmentRel> builder)
        {
            builder.HasKey(x => new { x.ProductId, x.AppoinmentId });

            builder.HasOne(x => x.Appointment)
                .WithMany(x => x.AppointmentServices)
                .HasForeignKey(x => x.AppoinmentId);

            builder.HasOne(x => x.Product)
                .WithMany()
                .HasForeignKey(x => x.ProductId);
        }
    }
}
