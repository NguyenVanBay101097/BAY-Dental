using ApplicationCore.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.EntityConfigurations
{
    public class DotKhamLineOperationConfiguration : IEntityTypeConfiguration<DotKhamLineOperation>
    {
        public void Configure(EntityTypeBuilder<DotKhamLineOperation> builder)
        {
            //builder.HasOne(x => x.Line)
            //    .WithMany(x => x.Operations)
            //    .HasForeignKey(x => x.LineId)
            //    .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Product)
             .WithMany()
             .HasForeignKey(x => x.ProductId);
        }
    }
}
