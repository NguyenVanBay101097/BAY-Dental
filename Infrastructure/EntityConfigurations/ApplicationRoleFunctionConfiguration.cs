using ApplicationCore.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.EntityConfigurations
{
    public class ApplicationRoleFunctionConfiguration : IEntityTypeConfiguration<ApplicationRoleFunction>
    {
        public void Configure(EntityTypeBuilder<ApplicationRoleFunction> builder)
        {
            builder.Property(x => x.Func).IsRequired();

            builder.HasOne(x => x.Role)
                .WithMany(x => x.Functions)
                .HasForeignKey(x => x.RoleId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.CreatedBy)
          .WithMany()
          .HasForeignKey(x => x.CreatedById);

            builder.HasOne(x => x.WriteBy)
                .WithMany()
                .HasForeignKey(x => x.WriteById);
        }
    }
}
