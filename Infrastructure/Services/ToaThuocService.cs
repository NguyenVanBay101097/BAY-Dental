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

        public async Task<ToaThuoc> GetToaThuocForDisplayAsync(Guid id)
        {
            return await SearchQuery(x => x.Id == id)
                .Include(x => x.User)
                .Include(x => x.Lines)
                .Include("Lines.Product")
                .FirstOrDefaultAsync();
        }

        public async Task<ToaThuocDisplay> DefaultGet(ToaThuocDefaultGet val)
        {
            var res = new ToaThuocDisplay();
            res.CompanyId = CompanyId;
            var userManager = GetService<UserManager<ApplicationUser>>();
            var user = await userManager.FindByIdAsync(UserId);
            res.UserId = user.Id;
            res.User = _mapper.Map<ApplicationUserSimple>(user);
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
                res.Partner = _mapper.Map<PartnerSimple>(partner);
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
            var toaThuocs = await SearchQuery(x => x.DotKhamId == dotKhamId).ToListAsync();
            var res = _mapper.Map<IEnumerable<ToaThuocBasic>>(toaThuocs);
            return res;
        }

        public async Task<ToaThuocPrintViewModel> GetToaThuocPrint(Guid id)
        {
            var toaThuoc = await SearchQuery(x => x.Id == id).Include(x => x.Company).Include(x => x.Company.Partner)
                .Include(x => x.Partner)
                .Include(x => x.Lines)
                .Include("Lines.Product").FirstOrDefaultAsync();
            var res = _mapper.Map<ToaThuocPrintViewModel>(toaThuoc);
            var partnerObj = GetService<IPartnerService>();
            res.CompanyAddress = partnerObj.GetFormatAddress(toaThuoc.Company.Partner);
            res.PartnerAddress = partnerObj.GetFormatAddress(toaThuoc.Partner);
            var now = DateTime.Now;
            res.PartnerAge = toaThuoc.Partner.BirthYear.HasValue ? (now.Year - toaThuoc.Partner.BirthYear.Value).ToString() : string.Empty;
            res.PartnerGender = partnerObj.GetGenderDisplay(toaThuoc.Partner);
            return res;
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

            var items = await _mapper.ProjectTo<ToaThuocBasic>(query.OrderByDescending(x => x.DateCreated).Skip(val.Offset).Take(val.Limit)).ToListAsync();
            var totalItems = await query.CountAsync();

            return new PagedResult2<ToaThuocBasic>(totalItems, val.Offset, val.Limit)
            {
                Items = items
            };
        }
    }
}
