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
    public class HrPayslipService : BaseService<HrPayslip>, IHrPayslipService
    {

        private readonly IMapper _mapper;
        public HrPayslipService(IAsyncRepository<HrPayslip> repository, IHttpContextAccessor httpContextAccessor, IMapper mapper)
           : base(repository, httpContextAccessor)
        {
            _mapper = mapper;
        }

        public async Task<HrPayslip> GetHrPayslipDisplay(Guid Id)
        {
            var res = await SearchQuery(x => x.Id == Id).Include(x => x.Struct).Include(x=>x.Employee).FirstOrDefaultAsync();
            return res;
        }

        public async Task<PagedResult2<HrPayslipDisplay>> GetPaged(HrPayslipPaged val)
        {
            var query = SearchQuery();
            if (!string.IsNullOrEmpty(val.Search))
            {
                query = query.Where(x => x.Name.Contains(val.Search));
            }
            query = query.Include(x => x.Struct);

            var items = await query.Skip(val.Offset).Take(val.Limit).ToListAsync();
            var totalItems = await query.CountAsync();
            return new PagedResult2<HrPayslipDisplay>(totalItems, val.Offset, val.Limit)
            {
                Items = _mapper.Map<IEnumerable<HrPayslipDisplay>>(items)
            };
        }
        //public async Task SaveLines(HrPayslipSave val, HrPayslip slip)
        //{
        //    var linesToRemove = new List<HrPayslipLine>();
        //    foreach (var line in slip.Lines)
        //    {
        //        if (!val.Lines.Any(x => x.Id == line.Id))
        //            linesToRemove.Add(line);
        //    }

        //    await GetService<IHrPayslipLineService>().Remove(linesToRemove.Select(x => x.Id).ToList());
        //    slip.Lines = slip.Lines.Except(linesToRemove).ToList();

        //    foreach (var line in val.Lines)
        //    {
        //        if (line.Id == Guid.Empty || !line.Id.HasValue)
        //        {
        //            var r = _mapper.Map<HrPayslipLine>(line);
        //            slip.Lines.Add(r);
        //        }
        //        else
        //        {
        //            _mapper.Map(line, slip.Lines.SingleOrDefault(c => c.Id == line.Id));
        //        }
        //    }
        //}
    }
}
