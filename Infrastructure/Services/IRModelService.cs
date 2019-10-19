using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using ApplicationCore.Models;
using ApplicationCore.Specifications;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class IRModelService : BaseService<IRModel>, IIRModelService
    {
        public IRModelService(IAsyncRepository<IRModel> repository, IHttpContextAccessor httpContextAccessor)
        : base(repository, httpContextAccessor)
        {
        }

        public async Task<PagedResult2<IRModel>> GetPagedAsync(int offset = 0, int limit = 10, string filter = "")
        {
            ISpecification<IRModel> spec = new InitialSpecification<IRModel>(x => true);
            if (!string.IsNullOrWhiteSpace(filter))
            {
                spec = spec.And(new InitialSpecification<IRModel>(x => x.Name.Contains(filter)));
            }

            // the implementation below using ForEach and Count. We need a List.
            var itemsOnPage = await SearchQuery(spec.AsExpression(), orderBy: x => x.OrderBy(s => s.Name), limit: limit, offSet: offset).ToListAsync();
            var totalItems = await CountAsync(spec);
            return new PagedResult2<IRModel>(totalItems, offset, limit)
            {
                Items = itemsOnPage
            };
        }
    }
}
