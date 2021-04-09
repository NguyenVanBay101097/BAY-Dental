using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using ApplicationCore.Models;
using ApplicationCore.Utilities;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Web.Models.ContentEditing;

namespace Infrastructure.Services
{
    public class SaleOrderPaymentService : BaseService<SaleOrderPayment>, ISaleOrderPaymentService
    {
        private readonly IMapper _mapper;

        public SaleOrderPaymentService(IAsyncRepository<SaleOrderPayment> repository, IHttpContextAccessor httpContextAccessor, IMapper mapper)
            : base(repository, httpContextAccessor)
        {
            _mapper = mapper;
        }

        public async Task<PagedResult2<SaleOrderPaymentBasic>> GetPagedResultAsync(SaleOrderPaymentPaged val)
        {
            var query = SearchQuery();


            if (val.SaleOrderId.HasValue)
            {
                query = query.Where(x => x.OrderId == val.SaleOrderId.Value);
            }


            var totalItems = await query.CountAsync();

            query = query.OrderByDescending(x => x.DateCreated);

            var items = await query.Skip(val.Offset).Take(val.Limit).ToListAsync();

            var paged = new PagedResult2<SaleOrderPaymentBasic>(totalItems, val.Offset, val.Limit)
            {
                Items = _mapper.Map<IEnumerable<SaleOrderPaymentBasic>>(items)
            };

            return paged;
        }

        public async Task<SaleOrderPaymentDisplay> GetDisplay(Guid id)
        {
            var paymentJournalLineObj = GetService<ISaleOrderPaymentJournalLineService>();
            var paymentHistoryLineObj = GetService<ISaleOrderPaymentHistoryLineService>();

            var saleOrderPaymentDisplay = await SearchQuery(x => x.Id == id).Select(x => new SaleOrderPaymentDisplay
            {
                Date = x.Date,
                Amount = x.Amount,
                Note = x.Note,
                State = x.State
            }).FirstOrDefaultAsync();

            saleOrderPaymentDisplay.JournalLines = await paymentJournalLineObj.SearchQuery(x => x.SaleOrderPaymentId == saleOrderPaymentDisplay.Id).Select(x => new SaleOrderPaymentJournalLineDisplay
            {
                Id = x.Id,
                JournalId = x.JournalId,
                Journal = new AccountJournalSimple
                {
                    Id = x.Journal.Id,
                    Name = x.Journal.Name
                },
                Amount = x.Amount

            }).ToListAsync();

            saleOrderPaymentDisplay.Lines = _mapper.Map<IEnumerable<SaleOrderPaymentHistoryLineDisplay>>(await paymentHistoryLineObj.SearchQuery(x => x.SaleOrderPaymentId == saleOrderPaymentDisplay.Id).Include(x => x.SaleOrderLine).ToListAsync());


            return saleOrderPaymentDisplay;
        }


        //public async Task<SaleOrderPaymentDisplay> SaleDefaultGet(IEnumerable<Guid> saleOrderIds)
        //{
        //    var orderObj = GetService<ISaleOrderService>();
        //    var orders = await orderObj.SearchQuery(x => saleOrderIds.Contains(x.Id) && x.Residual > 0).ToListAsync();

        //    if (!orders.Any())
        //        throw new Exception("Phiếu điều trị đã thanh toán đủ");

        //    if (orders.Any(x => x.State != "sale" && x.State != "done"))
        //        throw new Exception("Bạn chỉ có thể thanh toán cho phiếu điều trị đã xác nhận");

        //    if (orders.Any(x => x.PartnerId != orders[0].PartnerId))
        //        throw new Exception("Để thanh toán nhiều phiếu điều trị cùng một lần, chúng phải có cùng khách hàng");

        //    var total_amount = orders.Sum(x => x.Residual);

        //    var saleLineObj = GetService<ISaleOrderLineService>();
        //    var paymentRels = await saleLineObj.SearchQuery(x => saleOrderIds.Contains(x.OrderId) && x.AmountResidual != 0)
        //        .Select(x => new SaleOrderPaymentHistoryLineDisplay
        //        {
        //            SaleOrderLineId = x.Id,
        //            AmountPaid = x.AmountPaid,
        //            AmountResidual = x.AmountResidual,
        //            Name = x.Name,
        //            PriceTotal = x.PriceTotal
        //        }).ToListAsync();

        //    var communication = string.Join(", ", orders.Select(x => x.Name));
        //    var rec = new SaleOrderPaymentDisplay
        //    {
        //        Amount = Math.Abs(total_amount ?? 0),
        //        or = saleOrderIds,
        //        SaleOrderLinePaymentRels = paymentRels
        //    };

        //    return rec;
        //}

        public async Task<SaleOrderPayment> CreateSaleOrderPayment(SaleOrderPaymentSave val)
        {
            //Mapper
            var saleOrderPayment = _mapper.Map<SaleOrderPayment>(val);
            saleOrderPayment.CompanyId = val.CompanyId;
            await CreateAsync(saleOrderPayment);

            return saleOrderPayment;
        }

    }
}
