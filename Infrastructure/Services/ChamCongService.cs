using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using ApplicationCore.Models;
using ApplicationCore.Specifications;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using NPOI.SS.Formula.Functions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Web.Models.ContentEditing;

namespace Infrastructure.Services
{
    public class ChamCongService : BaseService<ChamCong>, IChamCongService
    {
        private readonly IMapper _mapper;
        public ChamCongService(IAsyncRepository<ChamCong> repository, IHttpContextAccessor httpContextAccessor, IMapper mapper)
           : base(repository, httpContextAccessor)
        {
            _mapper = mapper;
        }

        public async Task CreateListChamcongs(IEnumerable<ChamCong> val)
        {
            var companyId = this.CompanyId;
            foreach (var item in val)
            {
                item.CompanyId = companyId;
            }
            await this.CreateAsync(val);
        }
        public async Task<PagedResult2<EmployeeDisplay>> GetByEmployeePaged(employeePaged val)
        {
            var empObj = GetService<IEmployeeService>();
            var query = empObj.SearchQuery();

            var items = await query.Skip(val.Offset).Take(val.Limit)
               .ToListAsync();
            foreach (var item in items)
            {
                var chamcongQuery = this.SearchQuery(x => x.EmployeeId == item.Id);
                if (val.From.HasValue)
                {
                    chamcongQuery = chamcongQuery.Where(x => x.TimeIn.Value.Date >= val.From.Value.Date);
                }
                if (val.To.HasValue)
                {
                    chamcongQuery = chamcongQuery.Where(x => x.TimeIn.Value.Date <= val.To.Value.Date);
                }
                if (!string.IsNullOrEmpty(val.Status))
                {
                    chamcongQuery = chamcongQuery.Where(x => x.Status == val.Status);
                }
                item.ChamCongs = await chamcongQuery.ToListAsync();

            }
            var totalItems = await query.CountAsync();

            return new PagedResult2<EmployeeDisplay>(totalItems, val.Offset, val.Limit)
            {
                Items = _mapper.Map<IEnumerable<EmployeeDisplay>>(items)
            };
        }

        public Guid GetCurrentCompanyId()
        {
            return CompanyId;
        }
        public async Task<ChamCongDisplay> GetByEmployeeId(Guid id, DateTime date)
        {
            var chamcong = await SearchQuery(x => x.EmployeeId == id
            && (x.TimeIn.Value.Date.Equals(date.Date) ||x.TimeOut.Value.Date.Equals(date.Date)))
                .FirstOrDefaultAsync();
            return _mapper.Map<ChamCongDisplay>(chamcong);
        }
    }
}
