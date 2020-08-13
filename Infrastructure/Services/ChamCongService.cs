using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using ApplicationCore.Models;
using ApplicationCore.Specifications;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using NPOI.HSSF.Record.PivotTable;
using NPOI.SS.Formula.Functions;
using OfficeOpenXml.FormulaParsing.Excel.Functions.DateTime;
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
        //public async Task<PagedResult2<EmployeeDisplay>> GetByEmployeePaged(employeePaged val)
        //{
        //    ISpecification<Employee> spec = new InitialSpecification<Employee>(x => true);
        //    if (!string.IsNullOrEmpty(val.Filter))
        //    {
        //        spec = spec.And(new InitialSpecification<Employee>(x => x.Name.Contains(val.Filter)));
        //    }

        //    var empObj = GetService<IEmployeeService>();
        //    var query = empObj.SearchQuery().Include(x => x.ChamCongs).Include("ChamCongs.WorkEntryType").
        //        Select(x => new Employee()
        //        {
        //            Id = x.Id,
        //            CompanyId = x.CompanyId,
        //            Name = x.Name,
        //            Ref = x.Ref,
        //            ChamCongs = x.ChamCongs.Where(y => (!val.From.HasValue || (y.TimeIn.Value.Date >= val.From.Value.Date || y.TimeOut.Value.Date >= val.From.Value.Date))
        //            &&
        //            (!val.To.HasValue ||
        //            (y.TimeIn.Value.Date <= val.To.Value.Date || y.TimeOut.Value.Date <= val.To.Value.Date))
        //            &&
        //            (string.IsNullOrEmpty(val.Status) ||
        //            (y.Status == val.Status))

        //        ).Select(y => new ChamCong()
        //        {
        //            Id = y.Id,
        //            DateCreated = y.DateCreated,
        //            Status = y.Status,
        //            WorkEntryTypeId = y.WorkEntryTypeId,
        //            Date = y.Date,
        //            TimeIn = y.TimeIn,
        //            TimeOut = y.TimeOut,
        //            WorkEntryType = y.WorkEntryType,
        //            HourWorked = y.HourWorked
        //        }).ToList()
        //        });

        //    var items = await query.Skip(val.Offset).Take(val.Limit)
        //       .ToListAsync();

        //    var totalItems = await query.CountAsync();
        //    var resItems = _mapper.Map<IEnumerable<EmployeeDisplay>>(items);
        //    resItems = await CountNgayCong(resItems);
        //    return new PagedResult2<EmployeeDisplay>(totalItems, val.Offset, val.Limit)
        //    {
        //        Items = resItems
        //    };
        //}

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
                }
                emp.SoNgayCong = Tongcong;
            }
            return lst;
        }

        public async Task<string> GetStatus(ChamCong val)
        {
            if (val.TimeIn.HasValue && val.TimeOut.HasValue)
            {
                return "done";
            }
            else if (!val.TimeOut.HasValue && !val.TimeIn.HasValue)
            {
                var workEntry = await GetService<IWorkEntryTypeService>().GetByIdAsync(val.WorkEntryTypeId);
                if (workEntry == null)
                {
                    throw new Exception("loại chấm công không tìm thấy");
                }
                if (workEntry.IsHasTimeKeeping)
                {
                    return "NP";
                }
                return "Initial";
            }
            return "process";
        }
        public async Task<ChamCongDisplay> GetByEmployeeId(Guid id, DateTime date)
        {
            var chamcong = await SearchQuery(x => x.EmployeeId == id
            && (x.TimeIn.Value.Date.Equals(date.Date) || x.TimeOut.Value.Date.Equals(date.Date))).Include(x => x.Employee)
                .Include(x => x.WorkEntryType)
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

        //public async Task<IEnumerable<ChamCongDisplay>> ExportFile(employeePaged val)
        //{
        //    int dateStart = 0;
        //    int dateEnd = 0;
        //    var listDateCC = new List<DateChamCong>();
        //    var emps = await GetByEmployeePaged(val);
        //    var dictEmp = new Dictionary<Guid, List<DateChamCong>>();
        //    if (val.From.HasValue)
        //        dateStart = val.From.Value.Day;
        //    if (val.To.HasValue)
        //        dateEnd = val.To.Value.Day;

        //    for (int i = dateStart; i <= dateEnd; i++)
        //    {
        //        foreach (var emp in emps.Items)
        //        {
        //            if (emp.ChamCongs.Count() > 0)
        //            {
        //                foreach (var cc in emp.ChamCongs)
        //                {
        //                    if (cc.DateCreated.HasValue && cc.DateCreated.Value.Day == i)
        //                    {
        //                        var dateCC = new DateChamCong();
        //                        dateCC.Date = cc.DateCreated.Value;
        //                        dateCC.ChamCong = cc;
        //                        listDateCC.Add(dateCC);
        //                    }
        //                }
        //                dictEmp.Add(emp.Id, listDateCC);
        //            }
        //            //else
        //            //{
        //            //    var datec = new DateChamCong();
        //            //    datec.Date = new DateTime(currentYear, currentMonth, i);
        //            //    dictEmp[emp].Add(datec);
        //            //}
        //        }
        //    }


        //    return null;
        //}

        public async Task<IEnumerable<ChamCongDisplay>> GetAll(employeePaged val)
        {
            var query = SearchQuery();
            if (val.From.HasValue)
            {
                query = query.Where(y => y.Date.Value.Date >= val.From.Value.Date);
            }

            if (val.To.HasValue)
            {
                query = query.Where(y => y.Date.Value.Date <= val.To.Value.Date);
            }

            if (!string.IsNullOrEmpty(val.Status))
            {
                query = query.Where(x => x.Status.Equals(val.Status));
            }

            if (val.EmployeeId.HasValue)
            {
                query = query.Where(x => x.EmployeeId == val.EmployeeId);
            }

            var res = await query.Include(x => x.WorkEntryType).ToListAsync();
            return _mapper.Map<IEnumerable<ChamCongDisplay>>(res);
        }
    }
}
