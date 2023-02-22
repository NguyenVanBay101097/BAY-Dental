using ApplicationCore.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.EntityConfigurations
{
    public class VFundBookConfiguration : IEntityTypeConfiguration<VFundBook>
    {
        public void Configure(EntityTypeBuilder<VFundBook> builder)
        {
            builder.ToView("VFundBooks");
            builder.HasNoKey();

            builder.HasOne(x => x.Journal)
                .WithMany()
                .HasForeignKey(x => x.JournalId);
        }
    }
}
