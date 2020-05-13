using ApplicationCore.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Web.Models.ContentEditing;

namespace Infrastructure.Services
{
    public interface IServiceCardOrderLineService: IBaseService<ServiceCardOrderLine>
    {
        void PrepareLines(IEnumerable<ServiceCardOrderLine> self);
        ServiceCardCard PrepareCard(ServiceCardOrderLine self);
        AccountMoveLine PrepareInvoiceLine(ServiceCardOrderLine self);
        Task<ServiceCardOrderLineOnChangeCardTypeResponse> OnChangeCardType(ServiceCardOrderLineOnChangeCardType val);
    }
}
