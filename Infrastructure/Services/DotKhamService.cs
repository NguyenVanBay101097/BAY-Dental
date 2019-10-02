using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using ApplicationCore.Models;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Web.Models.ContentEditing;

namespace Infrastructure.Services
{
    public class DotKhamService : BaseService<DotKham>, IDotKhamService
    {
        private readonly IMapper _mapper;
        public DotKhamService(IAsyncRepository<DotKham> repository, IHttpContextAccessor httpContextAccessor,
            IMapper mapper)
        : base(repository, httpContextAccessor)
        {
            _mapper = mapper;
        }

        public async Task<DotKham> GetDotKhamForDisplayAsync(Guid id)
        {
            return await SearchQuery(x => x.Id == id)
                .Include(x => x.Partner)
                .Include(x=>x.Doctor)
                .Include(x => x.Assistant)
                .Include(x => x.User)
                .Include(x => x.Invoice)
                .Include(x => x.Lines)
                .Include(x => x.Appointment)
                .Include("Lines.Product")
                .Include("Lines.Operations")
                .Include("Lines.Operations.Product")
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<DotKham>> GetDotKhamsForInvoice(Guid invoiceId)
        {
            return await SearchQuery(x => x.InvoiceId == invoiceId, orderBy: x => x.OrderByDescending(s => s.DateCreated))
                .Include(x => x.User)
                .Include(x=>x.Doctor)
                .Include(x=>x.Assistant)
                .ToListAsync();
        }


        public async override Task<DotKham> CreateAsync(DotKham entity)
        {
            var sequenceService = (IIRSequenceService)_httpContextAccessor.HttpContext.RequestServices.GetService(typeof(IIRSequenceService));
            entity.Name = await sequenceService.NextByCode("dot.kham");
            if (string.IsNullOrEmpty(entity.Name) || entity.Name == "/")
            {
                await InsertDotKhamSequence();
                entity.Name = await sequenceService.NextByCode("dot.kham");
            }


            await base.CreateAsync(entity);
            //await ActionConfirm(entity.Id);
            return entity;
        }

        public async Task ActionConfirm(Guid id)
        {
            var dotKham = await SearchQuery(x => x.Id == id)
                .Include(x => x.Invoice)
                .Include("Invoice.InvoiceLines")
                .FirstOrDefaultAsync();
            var routingObj = GetService<IRoutingService>();
            var dotKhamStepObj = GetService<IDotKhamStepService>();
            var productStepObj = GetService<IProductStepService>();
            var dotKhamSteps = new List<DotKhamStep>();

            dotKham.State = "confirmed";
            await UpdateAsync(dotKham);            
        }

        public async Task<PagedResult2<DotKham>> GetPagedResultAsync(DotKhamPaged val)
        {
            var query = SearchQuery();
            if (!string.IsNullOrEmpty(val.Search))
                query = query.Where(x => x.Name.Contains(val.Search) || x.Partner.Name.Contains(val.Search) ||
                x.Partner.NameNoSign.Contains(val.Search));
            var items = await query.Include(x => x.User).Include(x => x.Partner).Include(x => x.Invoice)
                .OrderByDescending(x => x.Date).Skip(val.Offset).Take(val.Limit)
                .ToListAsync();
            var totalItems = await query.CountAsync();

            return new PagedResult2<DotKham>(totalItems, val.Offset, val.Limit)
            {
                Items = items
            };
        }

        public async Task<DotKhamDisplay> DefaultGet(DotKhamDefaultGet val)
        {
            var res = new DotKhamDisplay();
            res.CompanyId = CompanyId;
            var userManager = GetService<UserManager<ApplicationUser>>();
            var user = await userManager.FindByIdAsync(UserId);
            res.UserId = user.Id;
            res.User = _mapper.Map<ApplicationUserSimple>(user);
            if (val.InvoiceId.HasValue)
            {
                var invoiceObj = GetService<IAccountInvoiceService>();
                var invoice = await invoiceObj.SearchQuery(x => x.Id == val.InvoiceId).FirstOrDefaultAsync();
                res.InvoiceId = invoice.Id;
                res.PartnerId = invoice.PartnerId;
                res.Invoice = _mapper.Map<AccountInvoiceCbx>(invoice);
                var partnerObj = GetService<IPartnerService>();
                var partner = partnerObj.SearchQuery(x => x.Id == invoice.PartnerId).FirstOrDefault();
                res.Partner = _mapper.Map<PartnerSimple>(partner);
            }
            return res;
        }

        public async Task<IEnumerable<AccountInvoiceCbx>> GetCustomerInvoices(Guid customerId)
        {
            var invServiceObj = GetService<IAccountInvoiceService>();
            var inv = await invServiceObj.SearchQuery(x => x.PartnerId == customerId).OrderByDescending(x=>x.LastUpdated).ToListAsync();
            var invCbx = _mapper.Map<IEnumerable<AccountInvoiceCbx>>(inv);
            return invCbx;
        }

        private async Task InsertDotKhamSequence()
        {
            var seqObj = GetService<IIRSequenceService>();
            await seqObj.CreateAsync(new IRSequence
            {
                Code = "dot.kham",
                Name = "Mã đợt khám",
                Prefix = "DK",
                Padding = 6,
            });
        }
    }
}
