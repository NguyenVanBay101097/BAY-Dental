using ApplicationCore.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.EntityConfigurations
{
    class PartnerOldNewInSaleOrderConfiguration : IEntityTypeConfiguration<PartnerOldNewInSaleOrder>
    {
        public void Configure(EntityTypeBuilder<PartnerOldNewInSaleOrder> builder)
        {
            builder.ToView("partner_old_new_in_sale_order");
            builder.HasNoKey();

            builder.HasOne(x => x.Partner)
                .WithMany()
                .HasForeignKey(x => x.PartnerId);

            builder.HasOne(x => x.SaleOrder)
                .WithMany()
                .HasForeignKey(x => x.SaleOrderId);

            builder.HasOne(x => x.Company)
               .WithMany()
               .HasForeignKey(x => x.CompanyId);
        }

    }
}
