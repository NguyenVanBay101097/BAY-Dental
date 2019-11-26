using ApplicationCore.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.EntityConfigurations
{
    public class ResPartnerBankConfiguration : IEntityTypeConfiguration<ResPartnerBank>
    {
        public void Configure(EntityTypeBuilder<ResPartnerBank> builder)
        {
            builder.Property(x => x.AccountNumber).IsRequired();

            builder.HasOne(x => x.Partner)
                .WithMany()
                .HasForeignKey(x => x.PartnerId);

            builder.HasOne(x => x.Bank)
                .WithMany()
                .HasForeignKey(x => x.BankId);

            builder.HasOne(x => x.Company)
                .WithMany()
                .HasForeignKey(x => x.CompanyId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
