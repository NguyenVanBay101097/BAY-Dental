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
    public class HrPayslipRunService : BaseService<HrPayslipRun>, IHrPayslipRunService
    {
        private readonly IMapper _mapper;

        public HrPayslipRunService(IAsyncRepository<HrPayslipRun> repository, IHttpContextAccessor httpContextAccessor, IMapper mapper)
            : base(repository, httpContextAccessor)
        {
            _mapper = mapper;
        }

        public async Task<PagedResult2<HrPayslipRunBasic>> GetPagedResultAsync(HrPayslipRunPaged val)
        {
            ISpecification<HrPayslipRun> spec = new InitialSpecification<HrPayslipRun>(x => true);
            if (!string.IsNullOrEmpty(val.Search))
                spec = spec.And(new InitialSpecification<HrPayslipRun>(x => x.Name.Contains(val.Search)));

            var query = SearchQuery(spec.AsExpression(), orderBy: x => x.OrderByDescending(s => s.DateCreated));
            var items = await _mapper.ProjectTo<HrPayslipRunBasic>(query.Skip(val.Offset).Take(val.Limit)).ToListAsync();
            var totalItems = await query.CountAsync();

            return new PagedResult2<HrPayslipRunBasic>(totalItems, val.Offset, val.Limit)
            {
                Items = items
            };
        }

        public async Task<HrPayslipRunDisplay> GetHrPayslipRunForDisplay(Guid id)
        {
            var res = await _mapper.ProjectTo<HrPayslipRunDisplay>(SearchQuery(x => x.Id == id)).FirstOrDefaultAsync();
            if (res == null)
                throw new NullReferenceException("PayslipRun not found");

            return res;
        }

        public async Task<HrPayslipRun> CreatePayslipRun(HrPayslipRunSave val)
        {
            var payslipRun = _mapper.Map<HrPayslipRun>(val);
            payslipRun.CompanyId = CompanyId;

            return await CreateAsync(payslipRun);
        }

        public async Task UpdatePayslipRun(Guid id, HrPayslipRunSave val)
        {
            var paySlipRun = await SearchQuery(x => x.Id == id).Include(x => x.Slips).FirstOrDefaultAsync();
            if (paySlipRun == null)
                throw new Exception("PayslipRun not found");

            paySlipRun = _mapper.Map(val, paySlipRun);           

            await UpdateAsync(paySlipRun);
        }

        public async Task ActionConfirm(PaySlipRunConfirmViewModel val)
        {

        }
    }

    public class PaySlipRunConfirmViewModel
    {
        public Guid PayslipRunId { get; set; }
        
        public Guid structureId { get; set; }

        public IEnumerable<Employee> Employees { get; set; } = new List<Employee>();
    }
}
