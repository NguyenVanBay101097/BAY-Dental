using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using ApplicationCore.Models;
using ApplicationCore.Specifications;
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
    public class ServiceCardTypeService : BaseService<ServiceCardType>, IServiceCardTypeService
    {
        private readonly IMapper _mapper;
        public ServiceCardTypeService(IAsyncRepository<ServiceCardType> repository, IHttpContextAccessor httpContextAccessor,
            IMapper mapper)
            : base(repository, httpContextAccessor)
        {
            _mapper = mapper;
        }

        public async Task<PagedResult2<ServiceCardTypeBasic>> GetPagedResultAsync(ServiceCardTypePaged val)
        {
            ISpecification<ServiceCardType> spec = new InitialSpecification<ServiceCardType>(x => x.Active);
            if (!string.IsNullOrEmpty(val.Search))
                spec = spec.And(new InitialSpecification<ServiceCardType>(x => x.Name.Contains(val.Search)));

            var query = SearchQuery(spec.AsExpression(), orderBy: x => x.OrderByDescending(s => s.DateCreated));

            var items = await _mapper.ProjectTo<ServiceCardTypeBasic>(query).ToListAsync();
            var totalItems = await query.CountAsync();

            return new PagedResult2<ServiceCardTypeBasic>(totalItems, val.Offset, val.Limit)
            {
                Items = items
            };
        }

        public async Task<ServiceCardType> CreateUI(ServiceCardTypeSave val)
        {
            var type = _mapper.Map<ServiceCardType>(val);
            type.CompanyId = CompanyId;

            var product = await CreateProduct(type);
            type.ProductId = product.Id;

            return await CreateAsync(type);
        }

        public async Task UpdateUI(Guid id, ServiceCardTypeSave val)
        {
            var type = await SearchQuery(x => x.Id == id).Include(x => x.Product).FirstOrDefaultAsync();
            type = _mapper.Map(val, type);

            if (type.Product != null)
            {
                type.Product.Name = val.Name;
                type.Product.NameNoSign = val.Name.RemoveSignVietnameseV2();
            }

            await UpdateAsync(type);
        }

        public DateTime GetPeriodEndDate(ServiceCardType self, DateTime? dStart = null)
        {
            if (dStart == null)
                dStart = DateTime.Today;
            var nb = self.NbrPeriod ?? 0;
            if (self.Period == "year")
                nb = nb * 12;
            return dStart.Value.AddMonths(nb);
        }

        public override ISpecification<ServiceCardType> RuleDomainGet(IRRule rule)
        {
            var companyId = CompanyId;
            switch (rule.Code)
            {
                case "service_card.service_card_type_comp_rule":
                    return new InitialSpecification<ServiceCardType>(x => !x.CompanyId.HasValue || x.CompanyId == companyId);
                default:
                    return null;
            }
        }

        public async Task<Product> CreateProduct(ServiceCardType self)
        {
            var productObj = GetService<IProductService>();
            var reward_string = self.Name;
            var categObj = GetService<IProductCategoryService>();
            var categ = await categObj.DefaultCategory();
            var uomObj = GetService<IUoMService>();
            var uom = await uomObj.SearchQuery().FirstOrDefaultAsync();

            var product = new Product()
            {
                PurchaseOK = false,
                SaleOK = false,
                Type = "service",
                Name = reward_string,
                UOMId = uom.Id,
                UOMPOId = uom.Id,
                CategId = categ.Id,
                ListPrice = 0,
            };

            await productObj.CreateAsync(product);
            return product;
        }
    }
}
