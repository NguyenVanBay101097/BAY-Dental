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
using System.Runtime.CompilerServices;
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
            ISpecification<Employee> spec = new InitialSpecification<Employee>(x => true);
            if (!string.IsNullOrEmpty(val.Filter))
            {
                spec = spec.And(new InitialSpecification<Employee>(x => x.Name.Contains(val.Filter)));
            }

            var empObj = GetService<IEmployeeService>();
            var query = empObj.SearchQuery().Include(x => x.ChamCongs).Include("ChamCongs.WorkEntryType").
                Select(x => new Employee()
                {
                    Id = x.Id,
                    CompanyId = x.CompanyId,
                    Name = x.Name,
                    Ref = x.Ref,
                    ChamCongs = x.ChamCongs.Where(y =>
                    (!val.From.HasValue ||
                    (y.TimeIn.Value.Date >= val.From.Value.Date || y.TimeOut.Value.Date >= val.From.Value.Date))
                    &&
                    (!val.To.HasValue ||
                    (y.TimeIn.Value.Date <= val.To.Value.Date || y.TimeOut.Value.Date <= val.To.Value.Date))
                    &&
                    (string.IsNullOrEmpty(val.Status) ||
                    (y.Status == val.Status))

                ).Select(y => new ChamCong(){
                    Id = y.Id,
                    DateCreated = y.DateCreated,
                    Status = y.Status,
                    WorkEntryTypeId = y.WorkEntryTypeId,
                    TimeIn = y.TimeIn,
                    TimeOut = y.TimeOut,
                    WorkEntryType = y.WorkEntryType,
                    HourWorked = y.HourWorked
                        }).ToList()
                });

            var items = await query.Skip(val.Offset).Take(val.Limit)
               .ToListAsync();

            var totalItems = await query.CountAsync();
            var resItems = _mapper.Map<IEnumerable<EmployeeDisplay>>(items);
            resItems = await CountNgayCong(resItems);
            return new PagedResult2<EmployeeDisplay>(totalItems, val.Offset, val.Limit)
            {
                Items = resItems
            };
        }

        public async Task<IEnumerable<EmployeeDisplay>> CountNgayCong(IEnumerable<EmployeeDisplay> lst)
        {
            var setupObj = GetService<ISetupChamcongService>();
            var setup = await setupObj.GetByCompanyId();
            //skipp if setup null
            if (setup == null)
            {
                return lst;
            }
            var diffTime = Math.Round(setup.DifferenceTime / 60, 2);
            var fromOne = setup.OneStandardWorkHour - diffTime;
            var fromHalf = setup.HalfStandardWorkHour - diffTime;
            double Tongcong = 0;
            foreach (var emp in lst)
            {
                Tongcong = 0;
                foreach (var chamcong in emp.ChamCongs)
                {
                    if (chamcong.HourWorked >= fromOne || (chamcong.WorkEntryType != null && chamcong.WorkEntryType.IsHasTimeKeeping))
                    {
                        Tongcong += 1;
                    }
                    else if (chamcong.HourWorked >= fromHalf)
                    {
                        Tongcong += 0.5;
                    }
                    emp.SoNgayCong = Tongcong;
                }
            }
            return lst;
        }

        public Guid GetCurrentCompanyId()
        {
            return CompanyId;
        }
        public async Task<ChamCongDisplay> GetByEmployeeId(Guid id, DateTime date)
        {
            var chamcong = await SearchQuery(x => x.EmployeeId == id
            && (x.TimeIn.Value.Date.Equals(date.Date) || x.TimeOut.Value.Date.Equals(date.Date))).Include(x => x.Employee)
                .Include(x=>x.WorkEntryType)
                .FirstOrDefaultAsync();
            return _mapper.Map<ChamCongDisplay>(chamcong);
        }

        public async Task<decimal> GetStandardWorkHour()
        {
            var st = await GetService<ISetupChamcongService>().GetByCompanyId(CompanyId);
            if (st == null)
            {
                throw new Exception("cần setup chấm công trước");
            }
            return st.OneStandardWorkHour;
        }

        public Task<IEnumerable<ChamCongDisplay>> ExportFile(employeePaged val)
        {
            return null;
        }
    }
}
