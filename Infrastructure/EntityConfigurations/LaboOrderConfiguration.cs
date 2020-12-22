using ApplicationCore.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.EntityConfigurations
{
    public class LaboOrderConfiguration : IEntityTypeConfiguration<LaboOrder>
    {
        public void Configure(EntityTypeBuilder<LaboOrder> builder)
        {
            builder.Property(x => x.Name).IsRequired();

            builder.HasOne(x => x.Partner)
             .WithMany()
             .HasForeignKey(x => x.PartnerId)
             .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Company)
                .WithMany()
                .HasForeignKey(x => x.CompanyId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.SaleOrderLine)
                .WithMany(x => x.Labos)
                .HasForeignKey(x => x.SaleOrderLineId);

            builder.HasOne(x => x.Customer)
             .WithMany()
             .HasForeignKey(x => x.CustomerId);

            builder.HasOne(x => x.AccountMove)
             .WithMany()
             .HasForeignKey(x => x.AccountMoveId);

            builder.HasOne(x => x.Product)
                 .WithMany()
                 .HasForeignKey(x => x.ProductId);

            builder.HasOne(x => x.LaboBiteJoint)
                 .WithMany()
                 .HasForeignKey(x => x.LaboBiteJointId);

            builder.HasOne(x => x.LaboBridge)
                 .WithMany()
                 .HasForeignKey(x => x.LaboBridgeId);

            builder.HasOne(x => x.LaboFinishLine)
                 .WithMany()
                 .HasForeignKey(x => x.LaboFinishLineId);

            builder.HasOne(x => x.CreatedBy)
            .WithMany()
            .HasForeignKey(x => x.CreatedById);

            builder.HasOne(x => x.WriteBy)
                .WithMany()
                .HasForeignKey(x => x.WriteById);
        }
    }
}
