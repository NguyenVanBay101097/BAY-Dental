using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using ApplicationCore.Models;
using ApplicationCore.Specifications;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Web.Models.ContentEditing;

namespace Infrastructure.Services
{
    public class ToaThuocService : BaseService<ToaThuoc>, IToaThuocService
    {
        private readonly IMapper _mapper;
        public ToaThuocService(IAsyncRepository<ToaThuoc> repository, IHttpContextAccessor httpContextAccessor,
            IMapper mapper)
        : base(repository, httpContextAccessor)
        {
            _mapper = mapper;
        }

        public async Task<ToaThuocDisplay> GetToaThuocForDisplayAsync(Guid id)
        {
            var toathuoc = await _mapper.ProjectTo<ToaThuocDisplay>(SearchQuery(x => x.Id == id)).FirstOrDefaultAsync();
            if (toathuoc == null)
                return null;

            var toaThuocLineObj = GetService<IToaThuocLineService>();
            toathuoc.Lines = await _mapper.ProjectTo<ToaThuocLineDisplay>(toaThuocLineObj.SearchQuery(x => x.ToaThuocId == id, orderBy: x => x.OrderBy(s => s.Sequence))).ToListAsync();

            return toathuoc;
        }

        public async Task<ToaThuocDisplay> DefaultGet(ToaThuocDefaultGet val)
        {
            var res = new ToaThuocDisplay();
            res.CompanyId = CompanyId;
            var employeeObj = GetService<IEmployeeService>();
            var userId = UserId;

            var employee = await employeeObj.SearchQuery(x => x.IsDoctor && x.UserId == userId).FirstOrDefaultAsync();
            if (employee != null)
            {
                res.EmployeeId = employee.Id;
                res.Employee = _mapper.Map<EmployeeBasic>(employee);
            }

            if (val.DotKhamId.HasValue)
            {
                var dkObj = GetService<IDotKhamService>();
                var dk = await dkObj.SearchQuery(x => x.Id == val.DotKhamId)
                    .FirstOrDefaultAsync();
                if (dk.PartnerId.HasValue)
                    res.PartnerId = dk.PartnerId.Value;
                res.DotKhamId = dk.Id;
                res.DotKham = _mapper.Map<DotKhamSimple>(dk);
            }

            if (val.PartnerId.HasValue)
            {
                var partnerObj = GetService<IPartnerService>();
                var partner = await partnerObj.GetByIdAsync(val.PartnerId);
                res.PartnerId = partner.Id;
                res.Partner = _mapper.Map<PartnerCustomerDonThuoc>(partner);
            }

            if (val.SaleOrderId.HasValue)
            {
                res.SaleOrderId = val.SaleOrderId;
            }

            return res;
        }

        public async Task<ToaThuocLineDisplay> LineDefaultGet(ToaThuocLineDefaultGet val)
        {
            var res = new ToaThuocLineDisplay();
            if (val.ProductId.HasValue)
            {
                var productObj = GetService<IProductService>();
                var product = await productObj.SearchQuery(x => x.Id == val.ProductId)
                    .FirstOrDefaultAsync();
                res.ProductId = product.Id;
                res.Product = _mapper.Map<ProductSimple>(product);
                res.Note = product.KeToaNote;
            }
            return res;
        }

        public async override Task<ToaThuoc> CreateAsync(ToaThuoc entity)
        {

            var sequenceService = (IIRSequenceService)_httpContextAccessor.HttpContext.RequestServices.GetService(typeof(IIRSequenceService));
            entity.Name = await sequenceService.NextByCode("toa.thuoc");
            if (string.IsNullOrEmpty(entity.Name) || entity.Name == "/")
            {
                await InsertToaThuocSequence();
                entity.Name = await sequenceService.NextByCode("toa.thuoc");
            }

            var lineObj = GetService<IToaThuocLineService>();
            lineObj.ComputeName(entity.Lines);

            return await base.CreateAsync(entity);
        }

        public async Task Write(ToaThuoc entity)
        {
            var lineObj = GetService<IToaThuocLineService>();
            lineObj.ComputeName(entity.Lines);

            await UpdateAsync(entity);
        }

        public async Task<IEnumerable<ToaThuocBasic>> GetToaThuocsForDotKham(Guid dotKhamId)
        {
            var toaThuocs = await _mapper.ProjectTo<ToaThuocBasic>(SearchQuery(x => x.DotKhamId == dotKhamId)).ToListAsync();
            return toaThuocs;
        }

        public async Task<ToaThuocPrintViewModel> GetToaThuocPrint(Guid id)
        {
            var toaThuoc = await _mapper.ProjectTo<ToaThuocPrintViewModel>(SearchQuery(x => x.Id == id)).FirstOrDefaultAsync();
            if (toaThuoc == null)
                return null;

            var toaThuocLineObj = GetService<IToaThuocLineService>();
            toaThuoc.Lines = await _mapper.ProjectTo<ToaThuocLinePrintViewModel>(toaThuocLineObj.SearchQuery(x => x.ToaThuocId == id, orderBy: x => x.OrderBy(s => s.Sequence))).ToListAsync();

            return toaThuoc;
        }

        private async Task InsertToaThuocSequence()
        {
            var seqObj = GetService<IIRSequenceService>();
            await seqObj.CreateAsync(new IRSequence
            {
                Code = "toa.thuoc",
                Name = "Mã toa thuốc",
                Prefix = "TT",
                Padding = 6,
            });
        }

        public override ISpecification<ToaThuoc> RuleDomainGet(IRRule rule)
        {
            var companyId = CompanyId;
            switch (rule.Code)
            {
                case "sale.toa_thuoc_comp_rule":
                    return new InitialSpecification<ToaThuoc>(x => x.CompanyId == companyId);
                default:
                    return null;
            }
        }

        public async Task<PagedResult2<ToaThuocBasic>> GetPagedResultAsync(ToaThuocPaged val)
        {
            var query = SearchQuery();
            if (!string.IsNullOrEmpty(val.Search))
                query = query.Where(x => x.Name.Contains(val.Search));

            if (val.PartnerId.HasValue)
                query = query.Where(x => x.PartnerId == val.PartnerId);
            if (val.SaleOrderId.HasValue)
            {
                query = query.Where(x => x.SaleOrder.Id == val.SaleOrderId);
            }

            if(val.Limit > 0)
            {
                query = query.Skip(val.Offset).Take(val.Limit);
            }

            var items = await _mapper.ProjectTo<ToaThuocBasic>(query.OrderByDescending(x => x.DateCreated)).ToListAsync();
            var totalItems = await query.CountAsync();

            return new PagedResult2<ToaThuocBasic>(totalItems, val.Offset, val.Limit)
            {
                Items = items
            };
        }
    }
}
