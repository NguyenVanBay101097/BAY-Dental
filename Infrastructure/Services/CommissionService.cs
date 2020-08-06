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
    public class CommissionService : BaseService<Commission>, ICommissionService
    {
        private readonly IMapper _mapper;
        public CommissionService(IAsyncRepository<Commission> repository, IHttpContextAccessor httpContextAccessor, IMapper mapper)
            : base(repository, httpContextAccessor)
        {
            _mapper = mapper;
        }

        public async Task<PagedResult2<CommissionBasic>> GetPagedResultAsync(CommissionPaged val)
        {
            ISpecification<Commission> spec = new InitialSpecification<Commission>(x => true);
            if (!string.IsNullOrEmpty(val.Search))
                spec = spec.And(new InitialSpecification<Commission>(x => x.Name.Contains(val.Search)));

            var query = SearchQuery(spec.AsExpression(), orderBy: x => x.OrderByDescending(s => s.DateCreated));
            var items = await _mapper.ProjectTo<CommissionBasic>(query.Skip(val.Offset).Take(val.Limit)).ToListAsync();
            var totalItems = await query.CountAsync();

            return new PagedResult2<CommissionBasic>(totalItems, val.Offset, val.Limit)
            {
                Items = items
            };
        }

        public async Task<CommissionDisplay> GetCommissionForDisplay(Guid id)
        {
            var res = await _mapper.ProjectTo<CommissionDisplay>(SearchQuery(x => x.Id == id)).Include(x => x.CommissionProductRules).FirstOrDefaultAsync();
            if (res == null)
                throw new NullReferenceException("Commission not found");
                   
            return res;
        }

        public async Task<Commission> CreateCommission(CommissionDisplay val)
        {
            var commission = _mapper.Map<Commission>(val);
            SaveProductRules(val, commission);

            return await CreateAsync(commission);
        }

        public async Task UpdateCommission(Guid id, CommissionDisplay val)
        {
            var commission = await SearchQuery(x => x.Id == id).Include(x => x.CommissionProductRules).FirstOrDefaultAsync();
            if (commission == null)
                throw new Exception("Bảng hoa hồng không tồn tại");

            commission = _mapper.Map(val, commission);

            SaveProductRules(val, commission);

            await UpdateAsync(commission);
        }

        private void SaveProductRules(CommissionDisplay val, Commission commission)
        {
            //remove line
            var lineToRemoves = new List<CommissionProductRule>();
            foreach (var existLine in commission.CommissionProductRules)
            {
                if (!val.CommissionProductRules.Any(x => x.Id == existLine.Id))
                    lineToRemoves.Add(existLine);
            }

            foreach (var line in lineToRemoves)
            {
                commission.CommissionProductRules.Remove(line);
            }


            foreach (var item in val.CommissionProductRules)
            {
                if (item.Id == Guid.Empty)
                {
                    commission.CommissionProductRules.Add(_mapper.Map<CommissionProductRule>(item));
                }
                else
                {
                    _mapper.Map(item, commission.CommissionProductRules.SingleOrDefault(c => c.Id == item.Id));
                }
            }

        }
    }
}
