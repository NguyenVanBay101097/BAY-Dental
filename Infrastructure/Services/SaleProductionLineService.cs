using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class SaleProductionLineService : BaseService<SaleProductionLine>, ISaleProductionLineService
    {
        private readonly IMapper _mapper;

        public SaleProductionLineService(IAsyncRepository<SaleProductionLine> repository, IHttpContextAccessor httpContextAccessor, IMapper mapper)
            : base(repository, httpContextAccessor)
        {
            _mapper = mapper;
        }


        public async Task ComputeQtyRequested(IEnumerable<Guid> ids)
        {
            var lines = await SearchQuery(x => ids.Contains(x.Id)).Include(x => x.ProductRequestLineRels).ThenInclude(x => x.ProductRequestLine).ThenInclude(x => x.Request).ToListAsync();

            var rels = lines.SelectMany(x => x.ProductRequestLineRels.Where(s => s.ProductRequestLine.Request.State != "draft")).ToList();
            var requestedQty_dict = rels.GroupBy(x => x.SaleProductionLineId).ToDictionary(x => x.Key, x => x.Sum(s => s.ProductRequestLine.ProductQty));

            foreach (var line in lines)
                line.QuantityRequested = requestedQty_dict.ContainsKey(line.Id) ? requestedQty_dict[line.Id] : 0;

            await UpdateAsync(lines);
        }
    }
}
