using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using ApplicationCore.Models;
using ApplicationCore.Specifications;
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
    public class PromotionProgramService : BaseService<PromotionProgram>, IPromotionProgramService
    {
        private readonly IMapper _mapper;
        public PromotionProgramService(IAsyncRepository<PromotionProgram> repository, IHttpContextAccessor httpContextAccessor,
            IMapper mapper)
            : base(repository, httpContextAccessor)
        {
            _mapper = mapper;
        }

        public async Task<PagedResult2<PromotionProgramBasic>> GetPagedResultAsync(PromotionProgramPaged val)
        {
            ISpecification<PromotionProgram> spec = new InitialSpecification<PromotionProgram>(x => x.Active);
            if (!string.IsNullOrEmpty(val.Search))
                spec = spec.And(new InitialSpecification<PromotionProgram>(x => x.Name.Contains(val.Search)));

            var query = SearchQuery(spec.AsExpression(), orderBy: x => x.OrderByDescending(s => s.DateCreated));

            var items = await query.Skip(val.Offset).Take(val.Limit).Select(x => new PromotionProgramBasic
            {
                Id = x.Id,
                Name = x.Name,
            }).ToListAsync();

            var totalItems = await query.CountAsync();
            return new PagedResult2<PromotionProgramBasic>(totalItems, val.Offset, val.Limit)
            {
                Items = items
            };
        }

        public async Task<PromotionProgram> CreateProgram(PromotionProgramSave val)
        {
            var program = _mapper.Map<PromotionProgram>(val);
            foreach (var companyId in val.CompanyIds)
                program.ProgramCompanyRels.Add(new PromotionProgramCompanyRel { CompanyId = companyId });
            foreach(var ruleVal in val.Rules)
            {
                var rule = _mapper.Map<PromotionRule>(ruleVal);
                await SaveRule(rule, ruleVal);

                program.Rules.Add(rule);
            }

            return await CreateAsync(program);
        }

        public async Task ToggleActive(IEnumerable<Guid> ids)
        {
            var self = await SearchQuery(x => ids.Contains(x.Id)).ToListAsync();
            foreach (var program in self)
                program.Active = !program.Active;

            await UpdateAsync(self);
        }

        public override ISpecification<PromotionProgram> RuleDomainGet(IRRule rule)
        {
            var companyId = CompanyId;
            switch (rule.Code)
            {
                case "sale.promotion_program_comp_rule":
                    return new InitialSpecification<PromotionProgram>(x => !x.ProgramCompanyRels.Any() || x.ProgramCompanyRels.Any(s => s.CompanyId == companyId));
                default:
                    return null;
            }
        }

        public async Task UpdateProgram(Guid id, PromotionProgramSave val)
        {
            var program = await SearchQuery(x => x.Id == id)
                .Include(x => x.ProgramCompanyRels).Include(x => x.Rules)
                .Include("Rules.RuleCategoryRels").Include("Rules.RuleProductRels").FirstOrDefaultAsync();
            program = _mapper.Map(val, program);

            program.ProgramCompanyRels.Clear();
            foreach (var companyId in val.CompanyIds)
                program.ProgramCompanyRels.Add(new PromotionProgramCompanyRel { CompanyId = companyId });

            var valRuleIds = val.Rules.Select(x => x.Id).ToList();
            var to_remove = program.Rules.Where(x => !valRuleIds.Contains(x.Id)).ToList();
            foreach (var r in to_remove)
                program.Rules.Remove(r);

            foreach (var ruleVal in val.Rules)
            {
                if (ruleVal.Id == Guid.Empty)
                {
                    var rule = _mapper.Map<PromotionRule>(ruleVal);
                    await SaveRule(rule, ruleVal);
                    program.Rules.Add(rule);
                }
                else
                {
                    var rule = program.Rules.FirstOrDefault(x => x.Id == ruleVal.Id);
                    rule = _mapper.Map(ruleVal, rule);
                    await SaveRule(rule, ruleVal);
                }
              
            }

            await UpdateAsync(program);
        }

        private async Task SaveRule(PromotionRule rule, PromotionProgramRuleSave ruleVal)
        {
            if (rule.DiscountType == "fixed_amount")
            {
                rule.DiscountFixedAmount = rule.DiscountFixedAmount ?? 0;
                rule.DiscountPercentage = 0;
            }
            else
            {
                rule.DiscountPercentage = rule.DiscountPercentage ?? 0;
                rule.DiscountFixedAmount = 0;
            }

            if (rule.DiscountApplyOn == "2_product_category")
            {
                rule.RuleCategoryRels.Clear();
                var categObj = GetService<IProductCategoryService>();
                var categs = await categObj.SearchQuery(x => ruleVal.CategoryIds.Contains(x.Id)).ToListAsync();
                foreach(var categ in categs)
                {
                    var applyOn = categ.Name;
                    var discountProduct = await GetOrCreateDiscountProduct(rule, applyOn);
                    rule.RuleCategoryRels.Add(new PromotionRuleProductCategoryRel { CategId = categ.Id, DiscountLineProductId = discountProduct.Id });
                }
            }
            else if (rule.DiscountApplyOn == "0_product_variant")
            {
                rule.RuleProductRels.Clear();
                var productObj = GetService<IProductService>();
                var products = await productObj.SearchQuery(x => ruleVal.ProductIds.Contains(x.Id)).ToListAsync();
                foreach (var product in products)
                {
                    var applyOn = product.Name;
                    var discountProduct = await GetOrCreateDiscountProduct(rule, applyOn);
                    rule.RuleProductRels.Add(new PromotionRuleProductRel { ProductId = product.Id, DiscountLineProductId = discountProduct.Id });
                }
            }
            else
            {
                var applyOn = "tổng tiền";
                var discountProduct = await GetOrCreateDiscountProduct(rule, applyOn);
                rule.DiscountLineProductId = discountProduct.Id;
            }
        }

        private async Task<Product> GetOrCreateDiscountProduct(PromotionRule self, string applyOn = "")
        {
            var discount_product_name = "";
            if (self.DiscountType == "fixed_amount")
                discount_product_name = $"Giảm {(self.DiscountFixedAmount ?? 0).ToString("n0")} {applyOn}";
            else
                discount_product_name = $"Giảm {(self.DiscountPercentage ?? 0)}% {applyOn}";
            var productObj = GetService<IProductService>();
            var product = await productObj.SearchQuery(x => x.Name == discount_product_name).FirstOrDefaultAsync();
            if (product == null)
            {
                var categObj = GetService<IProductCategoryService>();
                var categ = await categObj.DefaultCategory();
                var uomObj = GetService<IUoMService>();
                var uom = await uomObj.SearchQuery(x => x.Name == "Chiết khấu").FirstOrDefaultAsync();
                if (uom == null)
                {
                    var uomCategObj = GetService<IUoMCategoryService>();
                    var uom_categ = await uomCategObj.SearchQuery(x => x.MeasureType == "unit").FirstOrDefaultAsync();
                    uom = new UoM
                    {
                        Name = "Chiết khấu",
                        CategoryId = uom_categ.Id,
                        MeasureType = uom_categ.MeasureType,
                    };
                    await uomObj.CreateAsync(uom);
                }

                product = new Product()
                {
                    PurchaseOK = false,
                    SaleOK = false,
                    Type = "service",
                    Name = discount_product_name,
                    UOMId = uom.Id,
                    UOMPOId = uom.Id,
                    CategId = categ.Id
                };
                await productObj.CreateAsync(product);
            }

            return product;
        }

        public async Task<PromotionProgramDisplay> GetDisplay(Guid id)
        {
            var res = await SearchQuery(x => x.Id == id).Select(x => new PromotionProgramDisplay
            {
                Id = x.Id,
                Companies = x.ProgramCompanyRels.Select(s => new CompanySimple {
                    Id = s.CompanyId,
                    Name = s.Company.Name
                }),
                DateFrom = x.DateFrom,
                DateTo = x.DateTo,
                Name = x.Name,
                MaximumUseNumber = x.MaximumUseNumber,
            }).FirstOrDefaultAsync();

            var ruleObj = GetService<IPromotionRuleService>();
            res.Rules = await ruleObj.SearchQuery(x => x.ProgramId == id).Select(x => new PromotionProgramRuleDisplay
            {
                Categories = x.RuleCategoryRels.Select(s => new ProductCategoryBasic {
                    Id = s.CategId,
                    CompleteName = s.Categ.CompleteName,
                    Name = s.Categ.Name,
                }),
                DiscountApplyOn = x.DiscountApplyOn,
                Id = x.Id,
                DiscountFixedAmount = x.DiscountFixedAmount,
                DiscountPercentage = x.DiscountPercentage,
                DiscountType = x.DiscountType,
                MinQuantity = x.MinQuantity,
                Products = x.RuleProductRels.Select(s => new ProductSimple
                {
                    Id = s.ProductId,
                    Name = s.Product.Name,
                })
            }).ToListAsync();

            return res;
        }
    }
}
