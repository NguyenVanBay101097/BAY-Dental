
using ApplicationCore.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace Infrastructure.EntityConfigurations
{
    class AccountFinancialRevenueReportAccountAccountRelConfiguration : IEntityTypeConfiguration<AccountFinancialRevenueReportAccountAccountRel>
    {
        public void Configure(EntityTypeBuilder<AccountFinancialRevenueReportAccountAccountRel> builder)
        {
            builder.HasKey(x => new { x.AccountId, x.FinancialReportId });

            builder.HasOne(x => x.Account)
                 .WithMany()
                 .HasForeignKey(x => x.AccountId);

            builder.HasOne(x => x.FinancialRevenueReport)
                .WithMany(x => x.FinancialRevenueReportAccountRels)
                .HasForeignKey(x => x.AccountId);

        }
    }
}