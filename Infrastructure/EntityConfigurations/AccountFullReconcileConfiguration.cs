using ApplicationCore.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.EntityConfigurations
{
    public class AccountFullReconcileConfiguration : IEntityTypeConfiguration<AccountFullReconcile>
    {
        public void Configure(EntityTypeBuilder<AccountFullReconcile> builder)
        {
            builder.Property(x => x.Name).IsRequired();

            builder.HasOne(x => x.ExchangeMove)
                .WithMany()
                .HasForeignKey(x => x.ExchangeMoveId);

            builder.HasOne(x => x.CreatedBy)
          .WithMany()
          .HasForeignKey(x => x.CreatedById);

            builder.HasOne(x => x.WriteBy)
                .WithMany()
                .HasForeignKey(x => x.WriteById);
        }
    }
}
