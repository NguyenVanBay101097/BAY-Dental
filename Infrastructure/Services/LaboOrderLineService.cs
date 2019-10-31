using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using ApplicationCore.Models;
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
    public class LaboOrderLineService : BaseService<LaboOrderLine>, ILaboOrderLineService
    {
        private readonly IMapper _mapper;

        public LaboOrderLineService(IAsyncRepository<LaboOrderLine> repository, IHttpContextAccessor httpContextAccessor,
            IMapper mapper)
        : base(repository, httpContextAccessor)
        {
            _mapper = mapper;
        }

        public override async Task<LaboOrderLine> CreateAsync(LaboOrderLine entity)
        {
            if (string.IsNullOrEmpty(entity.Name) || entity.Name == "/")
            {
                var sequenceService = (IIRSequenceService)_httpContextAccessor.HttpContext.RequestServices.GetService(typeof(IIRSequenceService));
                entity.Name = await sequenceService.NextByCode("labo.order.line");
            }
            return await base.CreateAsync(entity);
        }

        public async Task<IEnumerable<LaboOrderLineBasic>> GetAllForDotKham(Guid dotKhamId)
        {
            //var lines = await SearchQuery(x => x.DotKhamId == dotKhamId).Include(x => x.Product)
            //    .Include(x => x.Customer).Include(x => x.Supplier).OrderByDescending(x => x.DateCreated).ToListAsync();
            //var res = _mapper.Map<IEnumerable<LaboOrderLineBasic>>(lines);
            //return res;
            return null;
        }

        public async Task<LaboOrderLine> GetLaboLineForDisplay(Guid id)
        {
            //return await SearchQuery(x => x.Id == id).Include(x => x.Product)
            //    .Include(x => x.Customer).Include(x => x.Supplier).FirstOrDefaultAsync();
            return null;
        }

        public async Task<PagedResult2<LaboOrderLineBasic>> GetPagedResultAsync(LaboOrderLinePaged val)
        {
            //var query = GetQueryPaged(val);

            //var items = await query.Include(x => x.Customer).Include(x => x.Supplier).Include(x => x.Product).Skip(val.Offset).Take(val.Limit)
            //    .ToListAsync();
            //var totalItems = await query.CountAsync();

            //return new PagedResult2<LaboOrderLineBasic>(totalItems, val.Offset, val.Limit)
            //{
            //    Items = _mapper.Map<IEnumerable<LaboOrderLineBasic>>(items)
            //};
            return null;
        }

        private IQueryable<LaboOrderLine> GetQueryPaged(LaboOrderLinePaged val)
        {
            //var query = SearchQuery();

            //if (!string.IsNullOrEmpty(val.Search))
            //    query = query.Where(x => x.Name.Contains(val.Search) ||
            //    x.Customer.Name.Contains(val.Search) ||
            //    x.Customer.NameNoSign.Contains(val.Search) ||
            //    x.Customer.Phone.Contains(val.Search));

            //if (!string.IsNullOrEmpty(val.SearchSupplier))
            //    query = query.Where(x => x.Supplier.Name.Contains(val.SearchSupplier) ||
            //    x.Supplier.Ref.Contains(val.SearchSupplier) ||
            //    x.Supplier.Phone.Contains(val.SearchSupplier));

            //if (!string.IsNullOrEmpty(val.SearchCustomer))
            //    query = query.Where(x => x.Customer.Name.Contains(val.SearchCustomer) ||
            //    x.Customer.Ref.Contains(val.SearchCustomer) ||
            //    x.Customer.Phone.Contains(val.SearchCustomer));

            //if (!string.IsNullOrEmpty(val.SearchProduct))
            //    query = query.Where(x => x.Product.Name.Contains(val.SearchProduct) ||
            //    x.Product.DefaultCode.Contains(val.SearchProduct));

            //if (val.CustomerId.HasValue)
            //    query = query.Where(x => x.CustomerId == val.CustomerId);
            //if (val.SupplierId.HasValue)
            //    query = query.Where(x => x.SupplierId == val.SupplierId);
            //if (val.ProductId.HasValue)
            //    query = query.Where(x => x.ProductId == val.ProductId);
            //if (val.SentDateFrom.HasValue)
            //    query = query.Where(x => x.SentDate >= val.SentDateFrom);
            //if (val.SentDateTo.HasValue)
            //{
            //    var sentDateTo = val.SentDateTo.Value.AddDays(1);
            //    query = query.Where(x => x.SentDate < sentDateTo);
            //}

            //if (val.ReceivedDateFrom.HasValue)
            //    query = query.Where(x => x.ReceivedDate >= val.ReceivedDateFrom);
            //if (val.ReceivedDateTo.HasValue)
            //{
            //    var receivedDateTo = val.ReceivedDateTo.Value.AddDays(1);
            //    query = query.Where(x => x.ReceivedDate < receivedDateTo);
            //}

            //query = query.OrderByDescending(s => s.DateCreated);
            //return query;
            return null;
        }

        public async Task<LaboOrderLineDisplay> DefaultGet(LaboOrderLineDefaultGet val)
        {
            var res = new LaboOrderLineDisplay();
            //res.CompanyId = CompanyId;
            //if (val.InvoiceId.HasValue)
            //{
            //    var invObj = GetService<IAccountInvoiceService>();
            //    var invoice = await invObj.SearchQuery(x => x.Id == val.InvoiceId).Include(x => x.Partner).FirstOrDefaultAsync();
            //    res.InvoiceId = invoice.Id;
            //    res.CustomerId = invoice.PartnerId;
            //    res.Customer = _mapper.Map<PartnerSimple>(invoice.Partner);
            //}

            //if (val.DotKhamId.HasValue)
            //{
            //    var dkObj = GetService<IDotKhamService>();
            //    var dk = await dkObj.SearchQuery(x => x.Id == val.DotKhamId).Include(x => x.Partner).FirstOrDefaultAsync();
            //    res.DotKhamId = dk.Id;
            //    if (dk.PartnerId.HasValue)
            //        res.CustomerId = dk.PartnerId.Value;
            //    if (dk.Partner != null)
            //        res.Customer = _mapper.Map<PartnerSimple>(dk.Partner);
            //}
            return res;
        }

        public void _ComputeAmount(IEnumerable<LaboOrderLine> self)
        {
            foreach (var line in self)
            {
                line.PriceSubtotal = line.PriceUnit * line.ProductQty;
                line.PriceTotal = line.PriceSubtotal;
            }
        }

        public void _ComputeQtyInvoiced(IEnumerable<LaboOrderLine> self)
        {
            foreach (var line in self)
            {
                decimal qty = 0;
                foreach (var invLine in line.InvoiceLines)
                {
                    if (invLine.Invoice.State != "cancel")
                    {
                        if (invLine.Invoice.Type == "in_invoice")
                            qty += invLine.Quantity;
                        else if (invLine.Invoice.Type == "in_refund")
                            qty -= invLine.Quantity;
                    }
                }

                line.QtyInvoiced = qty;
            }
        }


        public async Task<LaboOrderLineOnChangeProductResult> OnChangeProduct(LaboOrderLineOnChangeProduct val)
        {
            var res = new LaboOrderLineOnChangeProductResult();
            if (val.ProductId.HasValue)
            {
                var productObj = GetService<IProductService>();
                res.PriceUnit = await productObj.GetStandardPrice(val.ProductId.Value);
            }

            return res;
        }
    }
}
