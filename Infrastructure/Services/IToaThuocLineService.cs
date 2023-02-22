using ApplicationCore.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using Umbraco.Web.Models.ContentEditing;

namespace Infrastructure.Services
{
    public interface IToaThuocLineService: IBaseService<ToaThuocLine>
    {
        void ComputeName(IEnumerable<ToaThuocLine> self);
        void ComputeQtyToInvoice(IEnumerable<ToaThuocLine> self);
    }
}
