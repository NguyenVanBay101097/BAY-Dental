using ApplicationCore.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.EntityConfigurations
{
    public class MedicineOrderConfiguration : IEntityTypeConfiguration<MedicineOrder>
    {



        public void Configure(EntityTypeBuilder<MedicineOrder> builder)
        {      

            builder.HasOne(x => x.Employee)
            .WithMany()
            .HasForeignKey(x => x.EmployeeId);

            builder.HasOne(x => x.ToaThuoc)
            .WithMany(x=>x.MedicineOrders)
            .HasForeignKey(x => x.ToathuocId);

            builder.HasOne(x => x.Partner)
          .WithMany()
          .HasForeignKey(x => x.PartnerId);

            builder.HasOne(x => x.AccountPayment)
           .WithMany()
           .HasForeignKey(x => x.AccountPaymentId);

            builder.HasOne(x => x.Journal)
              .WithMany()
              .HasForeignKey(x => x.JournalId);

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
