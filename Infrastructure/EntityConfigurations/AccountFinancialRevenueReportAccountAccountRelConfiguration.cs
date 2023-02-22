
using ApplicationCore.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace Infrastructure.EntityConfigurations
{
    class AccountFinancialRevenueReportAccountAccountRelConfiguration : IEntityTypeConfiguration<AccountFinancialRevenueReportAccountAccountRel>
    {
        public void Configure(EntityTypeBuilder<AccountFinancialRevenueReportAccountAccountRel> builder)
        {
            builder.HasOne(x => x.FinancialRevenueReport)
                .WithMany(x => x.FinancialRevenueReportAccountRels)
                .HasForeignKey(x => x.FinancialRevenueReportId);

            builder.HasOne(x => x.CreatedBy)
            .WithMany()
            .HasForeignKey(x => x.CreatedById);

            builder.HasOne(x => x.WriteBy)
                .WithMany()
                .HasForeignKey(x => x.WriteById);
        }
    }
}