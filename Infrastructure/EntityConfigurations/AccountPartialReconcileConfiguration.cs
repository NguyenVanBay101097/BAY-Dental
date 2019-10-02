using ApplicationCore.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.EntityConfigurations
{
    public class AccountPartialReconcileConfiguration : IEntityTypeConfiguration<AccountPartialReconcile>
    {
        public void Configure(EntityTypeBuilder<AccountPartialReconcile> builder)
        {
            builder.HasOne(x => x.CreditMove)
              .WithMany(x => x.MatchedDebits)
              .HasForeignKey(x => x.CreditMoveId)
              .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.DebitMove)
             .WithMany(x => x.MatchedCredits)
             .HasForeignKey(x => x.DebitMoveId)
             .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Company)
             .WithMany()
             .HasForeignKey(x => x.CompanyId);

            builder.HasOne(x => x.FullReconcile)
           .WithMany(x => x.PartialReconciles)
           .HasForeignKey(x => x.FullReconcileId);

            builder.HasOne(x => x.CreatedBy)
          .WithMany()
          .HasForeignKey(x => x.CreatedById);

            builder.HasOne(x => x.WriteBy)
                .WithMany()
                .HasForeignKey(x => x.WriteById);
        }
    }
}
