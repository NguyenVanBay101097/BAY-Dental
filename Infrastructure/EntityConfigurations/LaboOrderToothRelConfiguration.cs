using ApplicationCore.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.EntityConfigurations
{
    public class LaboOrderToothRelConfiguration : IEntityTypeConfiguration<LaboOrderToothRel>
    {
        public void Configure(EntityTypeBuilder<LaboOrderToothRel> builder)
        {
            builder.HasKey(x => new { x.ToothId, x.LaboOrderId });

            builder.HasOne(x => x.LaboOrder)
                .WithMany(x=> x.LaboOrderToothRel)
                .HasForeignKey(x => x.LaboOrderId);

            builder.HasOne(x => x.Tooth)
                .WithMany()
                .HasForeignKey(x => x.ToothId);
        }
    }
}
