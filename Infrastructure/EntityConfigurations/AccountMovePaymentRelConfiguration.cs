using ApplicationCore.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.EntityConfigurations
{
    public class AccountMovePaymentRelConfiguration : IEntityTypeConfiguration<AccountMovePaymentRel>
    {
        public void Configure(EntityTypeBuilder<AccountMovePaymentRel> builder)
        {
            builder.HasKey(x => new { x.PaymentId, x.MoveId });

            builder.HasOne(x => x.Payment)
                .WithMany(x => x.AccountMovePaymentRels)
                .HasForeignKey(x => x.PaymentId);

            builder.HasOne(x => x.Move)
                .WithMany()
                .HasForeignKey(x => x.MoveId);
        }
    }
}
