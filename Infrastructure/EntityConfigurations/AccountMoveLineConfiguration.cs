using ApplicationCore.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.EntityConfigurations
{
    public class AccountMoveLineConfiguration : IEntityTypeConfiguration<AccountMoveLine>
    {
        public void Configure(EntityTypeBuilder<AccountMoveLine> builder)
        {
            builder.Property(x => x.Name).IsRequired();

            builder.HasOne(x => x.Journal)
               .WithMany()
               .HasForeignKey(x => x.JournalId)
               .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Account)
              .WithMany()
              .HasForeignKey(x => x.AccountId)
              .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Move)
              .WithMany(x => x.Lines)
              .HasForeignKey(x => x.MoveId)
              .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.Partner)
                .WithMany(x => x.AMoveLines)
                .HasForeignKey(x => x.PartnerId);

            builder.HasOne(x => x.Company)
                .WithMany()
                .HasForeignKey(x => x.CompanyId);

            builder.HasOne(x => x.FullReconcile)
                .WithMany(x => x.ReconciledLines)
                .HasForeignKey(x => x.FullReconcileId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasOne(x => x.Payment)
                .WithMany(x => x.MoveLines)
                .HasForeignKey(x => x.PaymentId);

            builder.HasOne(x => x.PurchaseLine)
               .WithMany(x => x.MoveLines)
               .HasForeignKey(x => x.PurchaseLineId)
               .OnDelete(DeleteBehavior.SetNull);

            builder.HasOne(x => x.LaboLine)
              .WithMany(x => x.MoveLines)
              .HasForeignKey(x => x.LaboLineId)
              .OnDelete(DeleteBehavior.SetNull);

            builder.HasOne(x => x.Invoice)
                .WithMany()
                .HasForeignKey(x => x.InvoiceId);

            builder.HasOne(x => x.Salesman)
                .WithMany()
                .HasForeignKey(x => x.SalesmanId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasOne(x => x.PhieuThuChi)
            .WithMany(x => x.MoveLines)
            .HasForeignKey(x => x.PhieuThuChiId);

            builder.HasOne(x => x.CreatedBy)
            .WithMany()
            .HasForeignKey(x => x.CreatedById);

            builder.HasOne(x => x.WriteBy)
                .WithMany()
                .HasForeignKey(x => x.WriteById);

            builder.HasOne(x => x.Employee)
         .WithMany()
         .HasForeignKey(x => x.EmployeeId);

            builder.HasOne(x => x.Assistant)
              .WithMany()
              .HasForeignKey(x => x.AssistantId);
        }
    }
}
