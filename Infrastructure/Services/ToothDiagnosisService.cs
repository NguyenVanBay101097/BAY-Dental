using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using ApplicationCore.Models;
using ApplicationCore.Specifications;
using ApplicationCore.Utilities;
using AutoMapper;
using Infrastructure.Data;
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
    public class ToothDiagnosisService : BaseService<ToothDiagnosis>, IToothDiagnosisService
    {
        private readonly IMapper _mapper;

        public ToothDiagnosisService(
            IAsyncRepository<ToothDiagnosis> repository,
            IHttpContextAccessor httpContextAccessor,
            IMapper mapper
        ) : base(repository, httpContextAccessor)
        {
            _mapper = mapper;
        }

        public async Task<PagedResult2<ToothDiagnosisBasic>> GetPagedResultAsync(ToothDiagnosisPaged val)
        {
            var query = SearchQuery();

            if (!string.IsNullOrEmpty(val.Search))
            {
                query = query.Where(x => x.Name.Contains(val.Search));
            }

            if (val.CompanyId.HasValue)
            {
                query = query.Where(x => x.CompanyId == val.CompanyId);
            }

            query = query.OrderByDescending(x => x.DateCreated);

            var totalItems = await query.CountAsync();

            var items = await query.Skip(val.Offset).Take(val.Limit).ToListAsync();

            var paged = new PagedResult2<ToothDiagnosisBasic>(totalItems, val.Offset, val.Limit)
            {
                Items = _mapper.Map<IEnumerable<ToothDiagnosisBasic>>(items)
            };

            return paged;
        }

        public async Task<IEnumerable<ToothDiagnosisBasic>> GetAutocompleteAsync(ToothDiagnosisPaged val)
        {
            ISpecification<ToothDiagnosis> spec = new InitialSpecification<ToothDiagnosis>(x => true);
            if (!string.IsNullOrEmpty(val.Search))
                spec = spec.And(new InitialSpecification<ToothDiagnosis>(x => x.Name.Contains(val.Search)));

            var query = SearchQuery(spec.AsExpression(), orderBy: x => x.OrderByDescending(s => s.DateCreated));
            var items = await _mapper.ProjectTo<ToothDiagnosisBasic>(query.Skip(val.Offset).Take(val.Limit)).ToListAsync();

            return items;
        }

        public async Task<ToothDiagnosis> CreateToothDiagnosis(ToothDiagnosisSave val)
        {
            var toothDiagnosis = _mapper.Map<ToothDiagnosis>(val);

            // Thêm dịch vụ tư vấn
            if (val.ProductIds.Any())
            {
                foreach (var productId in val.ProductIds)
                {
                    toothDiagnosis.ToothDiagnosisProductRels.Add(new ToothDiagnosisProductRel() { ProductId = productId });
                }
            }

            await CreateAsync(toothDiagnosis);

            return toothDiagnosis;
        }

        public async Task UpdateToothDiagnosis(Guid id, ToothDiagnosisSave val)
        {
            var toothDiagnosis = await SearchQuery(x => x.Id == id)
                .Include(x => x.ToothDiagnosisProductRels).ThenInclude(x => x.Product)
                .FirstOrDefaultAsync();
            toothDiagnosis = _mapper.Map(val, toothDiagnosis);

            // Xóa dịch vụ tư vấn
            toothDiagnosis.ToothDiagnosisProductRels.Clear();
            // Thêm dịch vụ tư vấn
            if (val.ProductIds.Any())
            {
                foreach (var productId in val.ProductIds)
                {
                    toothDiagnosis.ToothDiagnosisProductRels.Add(new ToothDiagnosisProductRel() { ProductId = productId });
                }
            }

            await UpdateAsync(toothDiagnosis);
        }

        public async Task RemoveToothDiagnosis(Guid id)
        {
            var advisory = await SearchQuery(x => x.Id == id)
                .Include(x => x.ToothDiagnosisProductRels)
                .FirstOrDefaultAsync();
            await DeleteAsync(advisory);
        }
    }
}
