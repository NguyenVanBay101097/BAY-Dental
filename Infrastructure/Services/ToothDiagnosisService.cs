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
            ISpecification<ToothDiagnosis> spec = new InitialSpecification<ToothDiagnosis>(x => true);
            if (!string.IsNullOrEmpty(val.Search))
                spec = spec.And(new InitialSpecification<ToothDiagnosis>(x => x.Name.Contains(val.Search)));

            var query = SearchQuery(spec.AsExpression(), orderBy: x => x.OrderByDescending(s => s.DateCreated));
            var items = await _mapper.ProjectTo<ToothDiagnosisBasic>(query.Skip(val.Offset).Take(val.Limit)).ToListAsync();
            var totalItems = await query.CountAsync();

            return new PagedResult2<ToothDiagnosisBasic>(totalItems, val.Offset, val.Limit)
            {
                Items = items
            };
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

        //public override async Task<IEnumerable<ToothDiagnosis>> CreateToothDiagnosis(IEnumerable<ToothDiagnosis> entities)
        //{
        //    await base.CreateAsync(entities);
        //    return entities;
        //}

        public override async Task UpdateAsync(IEnumerable<ToothDiagnosis> entities)
        {
            await base.UpdateAsync(entities);
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
