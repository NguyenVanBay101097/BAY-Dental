using ApplicationCore.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Services
{
    public interface IServiceCardOrderLineService: IBaseService<ServiceCardOrderLine>
    {
        void PrepareLines(IEnumerable<ServiceCardOrderLine> self);
        ServiceCardCard PrepareCard(ServiceCardOrderLine self);
        AccountMoveLine PrepareInvoiceLine(ServiceCardOrderLine self);
    }
}
