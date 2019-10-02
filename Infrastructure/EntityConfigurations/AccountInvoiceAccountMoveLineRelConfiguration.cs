using ApplicationCore.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.EntityConfigurations
{
    public class AccountInvoiceAccountMoveLineRelConfiguration : IEntityTypeConfiguration<AccountInvoiceAccountMoveLineRel>
    {
        public void Configure(EntityTypeBuilder<AccountInvoiceAccountMoveLineRel> builder)
        {
            builder.HasKey(x => new { x.AccountInvoiceId, x.MoveLineId });

            builder.HasOne(x => x.AccountInvoice)
              .WithMany(x => x.PaymentMoveLines)
              .HasForeignKey(x => x.AccountInvoiceId);

            builder.HasOne(x => x.MoveLine)
             .WithMany()
             .HasForeignKey(x => x.MoveLineId);
        }
    }
}
