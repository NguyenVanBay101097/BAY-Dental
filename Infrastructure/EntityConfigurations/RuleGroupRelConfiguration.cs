using ApplicationCore.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.EntityConfigurations
{
    public class RuleGroupRelConfiguration : IEntityTypeConfiguration<RuleGroupRel>
    {
        public void Configure(EntityTypeBuilder<RuleGroupRel> builder)
        {
            builder.HasKey(x => new { x.RuleId, x.GroupId });

            builder.HasOne(x => x.Rule)
             .WithMany(x => x.RuleGroupRels)
             .HasForeignKey(x => x.RuleId);

            builder.HasOne(x => x.Group)
              .WithMany(x => x.RuleGroupRels)
              .HasForeignKey(x => x.GroupId);
        }
    }
}
