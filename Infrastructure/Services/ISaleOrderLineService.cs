using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ApplicationCore.Entities;
using Umbraco.Web.Models.ContentEditing;

namespace Infrastructure.Services
{
    public interface ISaleOrderLineService
    {
        void ComputeAmount(ICollection<SaleOrderLine> orderLines);

        /// <summary>
        /// Cập nhật 1 số thông tin từ sale order vào sale order line như company, salesman...
        /// </summary>
        /// <param name="orderLines"></param>
        /// <param name="order"></param>
        void UpdateOrderInfo(ICollection<SaleOrderLine> orderLines, SaleOrder order);
        Task<SaleOrderLineDisplay> OnChangeProduct(SaleOrderLineDisplay val);
    }
}
