using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using ApplicationCore.Models;
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
    public class HrPayslipLineService : BaseService<HrPayslipLine>, IHrPayslipLineService
    {

        private readonly IMapper _mapper;
        public HrPayslipLineService(IAsyncRepository<HrPayslipLine> repository, IHttpContextAccessor httpContextAccessor, IMapper mapper)
           : base(repository, httpContextAccessor)
        {
            _mapper = mapper;
        }

        public async Task<HrPayslipLine> GetHrPayslipLineDisplay(Guid Id)
        {
            var res = await SearchQuery(x => x.Id == Id).Include(x => x.Slip).FirstOrDefaultAsync();
            return res;
        }

        public async Task<PagedResult2<HrPayslipLineDisplay>> GetPaged(HrPayslipLinePaged val)
        {
            var query = SearchQuery();
            if (!string.IsNullOrEmpty(val.Search))
            {
                query = query.Where(x => x.Name.Contains(val.Search));
            }
            if (val.payslipId.HasValue)
            {
                query = query.Where( x =>x.SlipId == val.payslipId);
            }

            var items = await query.Skip(val.Offset).Take(val.Limit).ToListAsync();
            var totalItems = await query.CountAsync();
            return new PagedResult2<HrPayslipLineDisplay>(totalItems, val.Offset, val.Limit)
            {
                Items = _mapper.Map<IEnumerable<HrPayslipLineDisplay>>(items)
            };
        }

        public async Task Remove(IEnumerable<Guid> Ids)
        {
            var list = await SearchQuery(x => Ids.Contains(x.Id)).ToListAsync();
            await DeleteAsync(list);
        }
    }
}
