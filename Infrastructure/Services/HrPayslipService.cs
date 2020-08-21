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

        public async Task<IEnumerable<HrPayslipLine>> ComputePayslipLine(Guid? EmployeeId, DateTime? DateFrom, DateTime? DateTo)
        {
            var res = new List<HrPayslipLine>();
            var chamCongObj = GetService<IChamCongService>();
            var empObj = GetService<IEmployeeService>();
            var structureObj = GetService<IHrPayrollStructureService>();
            var emp = await empObj.SearchQuery(x => x.Id == EmployeeId.Value).FirstOrDefaultAsync();
            var structure = await structureObj.SearchQuery(x => x.TypeId == emp.StructureTypeId).Include(x => x.Rules).FirstOrDefaultAsync();
            var rules = structure.Rules.ToList();
            var CongEmp = await chamCongObj.CountCong(emp.Id, DateFrom, DateTo);
            var amoutADay = emp.Wage / CongEmp.CongChuan1Thang;
            for (int i = 0; i < rules.Count(); i++)
            {
                var rule = rules[i];
                var line = new HrPayslipLine();
                line.SalaryRuleId = rule.Id;
                line.Name = rule.Name;
                line.Code = rule.Code;
                line.Sequence = i;
                switch (rule.AmountSelect)
                {
                    case "code":
                        switch (rule.AmountCodeCompute)
                        {
                            //Tinh luong chinh cua Emp = so ngay cong * luong co ban
                            case "LC":
                                line.Amount = amoutADay * CongEmp.SoCong;
                                line.Total = line.Amount;
                                break;
                            //Hoa hong get tu service hoa hong
                            case "HH":
                                    
                                break;

                            default:
                                break;
                        }
                        break;

                    case "fixamount":
                        line.Amount = rule.AmountFix;
                        line.Total = rule.AmountFix;
                        break;

                    case "percent":

                        switch (rule.AmountPercentageBase)
                        {
                            //% dua vao luong chinh
                            case "LC":
                                line.Amount = (amoutADay * CongEmp.SoCong) * (rule.AmountPercentage / 100);
                                line.Total = (amoutADay * CongEmp.SoCong) * (rule.AmountPercentage / 100);
                                break;
                            //% dua vao hoa hong
                            case "HH":

                                break;

                            default:
                                break;
                        }
                        break;

                    default:
                        break;
                }
                res.Add(line);
            }
            return res;
        }

        public async Task<HrPayslip> GetHrPayslipDisplay(Guid Id)
        {
            var res = await SearchQuery(x => x.Id == Id).Include(x => x.Struct).Include(x=>x.Employee).Include(x=>x.Lines).FirstOrDefaultAsync();
            return res;
        }

        public async Task<PagedResult2<HrPayslipDisplay>> GetPaged(HrPayslipPaged val)
        {
            var query = SearchQuery();
            if (!string.IsNullOrEmpty(val.Search))
            {
                query = query.Where(x => x.Name.Contains(val.Search));
            }
            query = query.Include(x => x.Struct).Include(x=>x.Employee).OrderByDescending(x=>x.DateCreated);

            var items = await query.Skip(val.Offset).Take(val.Limit).ToListAsync();
            var totalItems = await query.CountAsync();
            return new PagedResult2<HrPayslipDisplay>(totalItems, val.Offset, val.Limit)
            {
                Items = _mapper.Map<IEnumerable<HrPayslipDisplay>>(items)
            };
        }
    }
}
