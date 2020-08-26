using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using ApplicationCore.Models;
using AutoMapper;
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

    public class HrPayslipWorkedDayService : BaseService<HrPayslipWorkedDays>, IHrPayslipWorkedDayService
    {

        private readonly IMapper _mapper;
        public HrPayslipWorkedDayService(IAsyncRepository<HrPayslipWorkedDays> repository, IHttpContextAccessor httpContextAccessor, IMapper mapper)
           : base(repository, httpContextAccessor)
        {
            _mapper = mapper;
        }

        public async Task<HrPayslipWorkedDays> GetHrPayslipWorkedDayDisplay(Guid Id)
        {
            var res = await SearchQuery(x => x.Id == Id).Include(x => x.Payslip).FirstOrDefaultAsync();
            return res;
        }

        public async Task<IEnumerable<HrPayslipWorkedDayDisplay>> GetPaged(HrPayslipWorkedDayPaged val)
        {
            var query = SearchQuery();
            if (!string.IsNullOrEmpty(val.Search))
            {
                query = query.Where(x => x.Name.Contains(val.Search));
            }

            if (val.PayslipId.HasValue)
            {
                query = query.Where(x => x.PayslipId == val.PayslipId);
            }
            query = query.Include(x => x.Payslip);

            var items = await query.ToListAsync();
            var totalItems = await query.CountAsync();

            return _mapper.Map<IEnumerable<HrPayslipWorkedDayDisplay>>(items);
        }

        public async Task Remove(IEnumerable<Guid> Ids)
        {
            var list = await SearchQuery(x => Ids.Contains(x.Id)).ToListAsync();
            await DeleteAsync(list);
        }
    }
}
