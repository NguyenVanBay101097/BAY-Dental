using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using ApplicationCore.Models;
using ApplicationCore.Specifications;
using AutoMapper;
using Facebook.ApiClient.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using NPOI.SS.Formula.Functions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Web.Models.ContentEditing;

namespace Infrastructure.Services
{
    public class WorkEntryTypeService : BaseService<WorkEntryType>, IWorkEntryTypeService
    {
        private readonly IMapper _mapper;
        public WorkEntryTypeService(IAsyncRepository<WorkEntryType> repository, IHttpContextAccessor httpContextAccessor, IMapper mapper)
           : base(repository, httpContextAccessor)
        {
            _mapper = mapper;
        }
        public async Task<PagedResult2<WorkEntryTypeDisplay>> GetPaged(WorkEntryTypePaged val)
        {
            ISpecification<WorkEntryType> spec = new InitialSpecification<WorkEntryType>(x => true);
            if (!string.IsNullOrEmpty(val.Filter))
                spec = spec.And(new InitialSpecification<WorkEntryType>(x => x.Name.Contains(val.Filter)));
            
            var query = SearchQuery(spec.AsExpression());
            var totalItems = await query.CountAsync();
            var items = await query.Skip(val.Offset).Take(val.Limit).ToListAsync();

            return new PagedResult2<WorkEntryTypeDisplay>(totalItems, val.Offset, val.Limit)
            {
                Items = _mapper.Map<IEnumerable<WorkEntryTypeDisplay>>(items)
            };
        }
    }
}
