using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using ApplicationCore.Models;
using ApplicationCore.Specifications;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using MyERP.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Web.Models.ContentEditing;

namespace Infrastructure.Services
{
    public class UoMService : BaseService<UoM>, IUoMService
    {

        private readonly IMapper _mapper;
        public UoMService(IAsyncRepository<UoM> repository, IHttpContextAccessor httpContextAccessor, IMapper mapper)
        : base(repository, httpContextAccessor)
        {
            _mapper = mapper;
        }

        public override Task<IEnumerable<UoM>> CreateAsync(IEnumerable<UoM> entities)
        {
            CheckRoundingAndFactor(entities);
            _UpdateProps(entities);
            return base.CreateAsync(entities);
        }

        public override Task UpdateAsync(IEnumerable<UoM> entities)
        {
            CheckRoundingAndFactor(entities);
            _UpdateProps(entities);
            return base.UpdateAsync(entities);
        }

        private void _UpdateProps(IEnumerable<UoM> self)
        {
            foreach(var uom in self)
            {
                if (string.IsNullOrEmpty(uom.MeasureType))
                {
                    if (uom.Category == null || (uom.Category.Id != uom.CategoryId && uom.CategoryId != Guid.Empty))
                    {
                        var categObj = GetService<IUoMCategoryService>();
                        uom.Category = categObj.GetById(uom.CategoryId);
                    }
                    uom.MeasureType = uom.Category.MeasureType;
                }
            }
        }

        public void CheckRoundingAndFactor(IEnumerable<UoM> self)
        {
            foreach (var uom in self)
            {
                if (uom.Rounding <= 0)
                    throw new Exception("Thuộc tính 'làm tròn' phải lớn hơn 0");

                if (uom.Factor == 0)
                    throw new Exception("Thuộc tính 'tỉ lệ' phải khác 0");
            }
        }

        public async Task<UoM> DefaultUOM()
        {
            var res = await SearchQuery().FirstOrDefaultAsync();
            return res;
        }

        public async Task<PagedResult2<UoMBasic>> GetPagedResultAsync(UoMPaged val)
        {
            ISpecification<UoM> spec = new InitialSpecification<UoM>(x => x.Active == true);
            if (!string.IsNullOrEmpty(val.Search))
                spec = spec.And(new InitialSpecification<UoM>(x => x.Name.Contains(val.Search)));

            if (val.CategoryId.HasValue)
                spec = spec.And(new InitialSpecification<UoM>(x => x.CategoryId == val.CategoryId));

            var query = SearchQuery(spec.AsExpression(), orderBy: x => x.OrderByDescending(s => s.DateCreated));
            var items = await _mapper.ProjectTo<UoMBasic>(query.Skip(val.Offset).Take(val.Limit)).ToListAsync();
            var totalItems = await query.CountAsync();
            return new PagedResult2<UoMBasic>(totalItems, val.Offset, val.Limit)
            {
                Items = items
            };
        }

        public decimal ComputePrice(UoM fromUOM, decimal price, UoM toUOM)
        {
            if (fromUOM == null || price == 0 || toUOM == null)
                return price;
            if (fromUOM.CategoryId != toUOM.CategoryId)
                return price;
            var amount = (double)price * fromUOM.Factor;
            if (toUOM != null)
                amount = amount / toUOM.Factor;
            return (decimal)amount;
        }

        public decimal ComputeQty(Guid fromUOMId, decimal qty, Guid toUOMId, string roundingMethod = "UP")
        {
            var fromUOM = GetById(fromUOMId);
            var toUOM = GetById(toUOMId);
            if (fromUOM == null || qty == 0 || toUOM == null)
            {
                return qty;
            }

            return ComputeQtyObj(fromUOM, qty, toUOM, roundingMethod: roundingMethod);
        }

        public decimal ComputeQtyObj(UoM fromUOM, decimal qty, UoM toUOM, string roundingMethod = "UP")
        {
            if (fromUOM.CategoryId != toUOM.CategoryId)
                throw new Exception("Conversion from Product UoM to Default UoM is not possible as they both belong to different Category!.");
            var amount = (double)qty / fromUOM.Factor;
            amount = amount * toUOM.Factor;
            amount = FloatUtils.FloatRound(amount, precisionRounding: (double)(toUOM.Rounding), roundingMethod: roundingMethod);
            return (decimal)amount;
        }
    }
}
