using ApplicationCore.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.EntityConfigurations
{
    class PhieuThuChiConfiguration : IEntityTypeConfiguration<PhieuThuChi>
    {
        public void Configure(EntityTypeBuilder<PhieuThuChi> builder)
        {
            builder.HasOne(x => x.LoaiThuChi)
              .WithMany()
              .HasForeignKey(x => x.LoaiThuChiId);

            builder.HasOne(x => x.Journal)
               .WithMany()
               .HasForeignKey(x => x.JournalId)
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
