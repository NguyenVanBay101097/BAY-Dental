using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using ApplicationCore.Models;
using ApplicationCore.Specifications;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Web.Models.ContentEditing;
using ApplicationCore.Utilities;
using OfficeOpenXml;

namespace Infrastructure.Services
{
    public class AppointmentService : BaseService<Appointment>, IAppointmentService
    {
        private readonly IMapper _mapper;
        private readonly IProductAppointmentRelService _productAppointmentRelService;
        public AppointmentService(IProductAppointmentRelService productAppointmentRelService, IAsyncRepository<Appointment> repository, IHttpContextAccessor httpContextAccessor,
            IMapper mapper)
        : base(repository, httpContextAccessor)
        {
            _mapper = mapper;
            _productAppointmentRelService = productAppointmentRelService;
        }

        public override async Task<Appointment> CreateAsync(Appointment entity)
        {
            var sequenceService = (IIRSequenceService)_httpContextAccessor.HttpContext.RequestServices.GetService(typeof(IIRSequenceService));
            entity.Name = await sequenceService.NextByCode("appointment");
            if (string.IsNullOrEmpty(entity.Name) || entity.Name == "/")
            {
                await InsertAppointmentSequence();
                entity.Name = await sequenceService.NextByCode("appointment");
            }
            if (!string.IsNullOrEmpty(entity.Time))
            {
                entity.DateTimeAppointment = entity.Date.Add(TimeSpan.Parse(entity.Time));
            }
            return await base.CreateAsync(entity);
        }

        public async Task<PagedResult2<Appointment>> GetPagedResultAsync(int offset = 0, int limit = 20, string search = "")
        {
            Expression<Func<Appointment, bool>> domain = x => string.IsNullOrEmpty(search) || x.Name.Contains(search);

            var query = SearchQuery(domain: domain, orderBy: x => x.OrderBy(s => s.Name), offSet: offset, limit: limit);
            var items = await query
                .Include(x => x.Partner).Include(x => x.User)
                .ToListAsync();
            var totalItems = await query.CountAsync();

            return new PagedResult2<Appointment>(totalItems, offset, limit)
            {
                Items = items
            };
        }

        public async Task<AppointmentDisplay> GetAppointmentDisplayAsync(Guid id)
        {
            var category = await SearchQuery(x => x.Id == id)
                .Include(x => x.User)
                .Include("Partner")
                .Include("Partner.PartnerPartnerCategoryRels")
                .Include("Partner.PartnerPartnerCategoryRels.Category")
                .Include(x => x.AppointmentServices).ThenInclude(x => x.Product)
                .Include("Doctor")
                .FirstOrDefaultAsync();
            //Xác định cuộc hẹn đã có đợt khám tham chiếu hay chưa
            var dotKhamObj = GetService<IDotKhamService>();
            var query = await dotKhamObj.SearchQuery(x => x.AppointmentId == id).ToListAsync();
            var res = _mapper.Map<AppointmentDisplay>(category);
            if (query.Count() > 0)
            {
                res.HasDotKhamRef = true;
            }
            return res;
        }

        public async Task<PagedResult2<AppointmentBasic>> GetPagedResultAsync(AppointmentPaged val)
        {
            var query = GetSearchQuery(search: val.Search, state: val.State, isLate: val.IsLate,
                partnerId: val.PartnerId, doctorId: val.DoctorId, dateFrom: val.DateTimeFrom,
                dateTo: val.DateTimeTo, userId: val.UserId, companyId: val.CompanyId, dotKhamId: val.DotKhamId);

            query = query.OrderByDescending(x => x.DateCreated);
            var totalItems = await query.CountAsync();
            var limit = val.Limit > 0 ? val.Limit : int.MaxValue;

            var items = await query
                .Include(x => x.Partner)
                .Include(x => x.Doctor)
                .Include(x => x.AppointmentServices).ThenInclude(x => x.Product)
                .OrderBy(x => x.Date).ThenBy(x => x.Time)
                .Skip(val.Offset)
                .Take(limit)
                .ToListAsync();

            return new PagedResult2<AppointmentBasic>(totalItems, val.Offset, limit)
            {
                Items = _mapper.Map<IEnumerable<AppointmentBasic>>(items)
            };
        }

        public async Task<AppointmentDisplay> DefaultGet(AppointmentDefaultGet val)
        {
            var res = new AppointmentDisplay();
            res.CompanyId = CompanyId;

            if (val.DotKhamId.HasValue)
            {
                var dkObj = GetService<IDotKhamService>();
                var dk = await dkObj.SearchQuery(x => x.Id == val.DotKhamId).Include(x => x.Partner)
                    .Include(x => x.Doctor).FirstOrDefaultAsync();

                res.DotKhamId = dk.Id;
                if (dk.PartnerId.HasValue)
                    res.PartnerId = dk.PartnerId.Value;
                if (dk.Partner != null)
                    res.Partner = _mapper.Map<PartnerSimpleInfo>(dk.Partner);

                if (dk.Doctor != null)
                {
                    res.DoctorId = dk.Doctor.Id;
                    res.Doctor = _mapper.Map<EmployeeSimple>(dk.Doctor);
                }
            }

            if (val.PartnerId.HasValue)
            {
                var partnerObj = GetService<IPartnerService>();
                var partner = await partnerObj.GetByIdAsync(val.PartnerId.Value);
                res.Partner = _mapper.Map<PartnerSimpleInfo>(partner);
            }

            if (val.SaleOrderId.HasValue)
            {
                res.SaleOrderId = val.SaleOrderId;
            }

            return res;
        }
        //Đếm số cuộc hẹn trong ngày (trang Tổng quan)
        public async Task<IEnumerable<AppointmentStateCount>> CountAppointment(DateFromTo val)
        {
            var AllCount = await SearchQuery().Where(x => x.Date >= val.DateFrom && x.Date <= val.DateTo).CountAsync();
            var confirmCount = await SearchQuery().Where(x => x.Date >= val.DateFrom && x.Date <= val.DateTo && x.State.Contains("confirmed")).CountAsync();
            var cancelCount = await SearchQuery().Where(x => x.Date >= val.DateFrom && x.Date <= val.DateTo && x.State.Contains("cancel")).CountAsync();
            var doneCount = await SearchQuery().Where(x => x.Date >= val.DateFrom && x.Date <= val.DateTo && x.State.Contains("done")).CountAsync();
            var waitingCount = await SearchQuery().Where(x => x.Date >= val.DateFrom && x.Date <= val.DateTo && x.State.Contains("waiting")).CountAsync();
            var expiredCount = await SearchQuery().Where(x => x.Date >= val.DateFrom && x.Date <= val.DateTo && x.State.Contains("examination")).CountAsync();

            var list = new List<AppointmentStateCount>();
            list.Add(new AppointmentStateCount { State = "all", Count = AllCount, Color = "#04c835" });
            list.Add(new AppointmentStateCount { State = "confirmed", Count = confirmCount, Color = "#04c835" });
            list.Add(new AppointmentStateCount { State = "waiting", Count = waitingCount, Color = "#0080ff" });
            list.Add(new AppointmentStateCount { State = "examination", Count = expiredCount, Color = "#ffbf00" });
            list.Add(new AppointmentStateCount { State = "done", Count = doneCount, Color = "#666666" });
            list.Add(new AppointmentStateCount { State = "cancel", Count = cancelCount, Color = "#cc0000" });

            return list;
        }

        public async Task<long> GetCount(AppointmentGetCountVM val)
        {
            var query = GetSearchQuery(state: val.State, dateFrom: val.DateFrom, dateTo: val.DateTo, isLate: val.IsLate, doctorId: val.DoctorId, search: val.Search);
            return await query.LongCountAsync();
        }

        public IQueryable<Appointment> GetSearchQuery(string search = "", Guid? partnerId = null,
            string state = "", DateTime? dateTo = null, DateTime? dateFrom = null, Guid? dotKhamId = null,
            Guid? doctorId = null, bool? isLate = null, string userId = "", Guid? companyId = null)
        {
            var query = SearchQuery();
            var today = DateTime.Today;
            if (isLate.HasValue)
            {
                if (isLate.Value)
                    query = query.Where(x => x.Date < today);
                else
                    query = query.Where(x => x.Date >= today);
            }

            if (!string.IsNullOrEmpty(search))
                query = query.Where(x => x.Name.Contains(search) || x.Doctor.Name.Contains(search)
                || x.Partner.Name.Contains(search) || x.Partner.Phone.Contains(search)
                || x.Partner.Ref.Contains(search));

            if (dateFrom.HasValue)
                query = query.Where(x => x.Date >= dateFrom.Value);

            if (dateTo.HasValue)
            {
                dateTo = dateTo.Value.AbsoluteEndOfDate();
                query = query.Where(x => x.Date <= dateTo);
            }

            if (companyId.HasValue)
                query = query.Where(x => x.CompanyId == companyId.Value);

            if (dotKhamId.HasValue)
                query = query.Where(x => x.DotKhamId == dotKhamId.Value);

            if (!string.IsNullOrEmpty(userId))
                query = query.Where(x => x.UserId == userId);

            string[] stateList = null;
            if (!string.IsNullOrEmpty(state))
            {
                stateList = state.Split(",");
                if (stateList.Contains("overdue"))
                {
                    query = query.Where(x => x.Date.Date < DateTime.Now.Date);
                    stateList = stateList.Where(x=> !x.Contains("overdue")).ToArray();
                } else
                {
                    query = query.Where(x => x.Date.Date >= DateTime.Now.Date);
                }
                if(stateList.Any())
                query = query.Where(x => stateList.Contains(x.State));
            }

            if (partnerId.HasValue)
                query = query.Where(x => x.PartnerId == partnerId.Value);

            if (doctorId.HasValue)
                query = query.Where(x => x.DoctorId == doctorId.Value);

            return query;
        }

        private async Task InsertAppointmentSequence()
        {
            var seqObj = GetService<IIRSequenceService>();
            await seqObj.CreateAsync(new IRSequence
            {
                Code = "appointment",
                Name = "Mã lịch hẹn",
                Prefix = "LH",
                Padding = 4,
            });
        }

        public override ISpecification<Appointment> RuleDomainGet(IRRule rule)
        {
            var companyId = CompanyId;
            switch (rule.Code)
            {
                case "sale.appointment_comp_rule":
                    return new InitialSpecification<Appointment>(x => x.CompanyId == companyId);
                default:
                    return null;
            }
        }

        public async Task<IEnumerable<AppointmentBasic>> SearchRead(AppointmentSearch val)
        {
            var query = SearchQuery();
            if (!string.IsNullOrEmpty(val.Search))
                query = query.Where(x => x.Name.Contains(val.Search) || x.Doctor.Name.Contains(val.Search)
                || x.Partner.Name.Contains(val.Search) || x.Partner.Phone.Contains(val.Search)
                || x.Partner.Ref.Contains(val.Search));

            if (val.DateTimeFrom.HasValue)
                query = query.Where(x => x.Date >= val.DateTimeFrom);

            if (val.DateTimeTo.HasValue)
                query = query.Where(x => x.Date < val.DateTimeTo.Value);

            if (val.DotKhamId.HasValue)
                query = query.Where(x => x.DotKhamId == val.DotKhamId);

            if (!string.IsNullOrEmpty(val.State))
            {
                var stateList = val.State.Split(",");
                query = query.Where(x => stateList.Contains(x.State));
            }

            query = query.OrderByDescending(x => x.DateCreated);
            var res = await query.Select(x => new AppointmentBasic
            {
                Id = x.Id,
                Name = x.Name,
                PartnerName = x.Partner.Name,
                DoctorName = x.Doctor.Name,
                Date = x.Date,
                State = x.State
            }).ToListAsync();

            return res;

        }

        public async Task<PagedResult2<AppointmentBasic>> SearchReadByDate(AppointmentSearchByDate val)
        {
            var query = SearchQuery();
            if (!string.IsNullOrEmpty(val.Search))
                query = query.Where(x => x.Name.Contains(val.Search)
                || x.Partner.Name.Contains(val.Search) || x.Partner.NameNoSign.Contains(val.Search) || x.Partner.Phone.Contains(val.Search)
                || x.Partner.Ref.Contains(val.Search));

            if (val.Date.HasValue)
            {
                var dateFrom = val.Date.Value;
                var dateTo = val.Date.Value.AbsoluteEndOfDate();
                query = query.Where(x => x.Date >= dateFrom && x.Date <= dateTo);
            }

            if (!string.IsNullOrEmpty(val.UserId))
                query = query.Where(x => x.UserId == val.UserId);

            if (val.EmployeeId.HasValue)
                query = query.Where(x => x.DoctorId == val.EmployeeId);

            if (!string.IsNullOrEmpty(val.State))
            {
                var stateList = val.State.Split(",");
                query = query.Where(x => stateList.Contains(x.State));
            }

            var totalItems = await query.CountAsync();

            query = query.OrderByDescending(x => x.DateCreated);
            var limit = val.Limit > 0 ? val.Limit : int.MaxValue;

            var items = await query
                .Include(x => x.Partner)
                .Include(x => x.Doctor)
                .OrderByDescending(x => x.DateCreated)
                .Skip(val.Offset)
                .Take(limit)
                .ToListAsync();

            return new PagedResult2<AppointmentBasic>(totalItems, val.Offset, limit)
            {
                Items = _mapper.Map<IEnumerable<AppointmentBasic>>(items)
            };
        }

        public async Task<AppointmentBasic> GetBasic(Guid id)
        {
            return await SearchQuery(x => x.Id == id).Select(x => new AppointmentBasic
            {
                Id = x.Id,
                Name = x.Name,
                PartnerName = x.Partner.Name,
                DoctorName = x.Doctor.Name,
                Date = x.Date,
                Time = x.Time,
                State = x.State,
                Note = x.Note,
                PartnerPhone = x.Partner.Phone,
                UserName = x.User.Name,
                PartnerDisplayName = x.Partner.DisplayName
            }).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<AppointmentBasic>> GetExcelData(AppointmentPaged val)
        {
            var query = GetSearchQuery(search: val.Search, state: val.State, isLate: val.IsLate,
              partnerId: val.PartnerId, doctorId: val.DoctorId, dateFrom: val.DateTimeFrom,
              dateTo: val.DateTimeTo, userId: val.UserId, companyId: val.CompanyId, dotKhamId: val.DotKhamId);

            query = query.OrderByDescending(x => x.DateCreated);

            var totalItems = await query.CountAsync();
            var items = await query
                .Include(x => x.Partner).Include("Partner.PartnerPartnerCategoryRels.Category")
                .Include(x => x.Doctor)
                .Include(x => x.AppointmentServices).ThenInclude(x => x.Product)
                .OrderBy(x => x.Date).ThenBy(x => x.Time)
                .ToListAsync();

            return _mapper.Map<IEnumerable<AppointmentBasic>>(items);
        }

        public async Task<Appointment> CreateAsync(AppointmentDisplay val)
        {
            var appointment = _mapper.Map<Appointment>(val);
            if (val.Services.Any())
            {
                foreach (var item in val.Services)
                {
                    appointment.AppointmentServices.Add(new ProductAppointmentRel()
                    {
                        ProductId = item.Id
                    });
                }
            }

            appointment = await CreateAsync(appointment);
            return appointment;
        }

        public async Task UpdateAsync(Guid id, AppointmentDisplay val)
        {
            var appointment = await SearchQuery(x => x.Id == id).Include(x => x.AppointmentServices).ThenInclude(x => x.Product).FirstOrDefaultAsync();
            await ComputeAppointmentService(appointment, val);
            appointment = _mapper.Map(val, appointment);
            if (!string.IsNullOrEmpty(val.Time) && val.Date.HasValue)
            {
                appointment.DateTimeAppointment = val.Date.Value.Add(TimeSpan.Parse(val.Time));
            }
            await UpdateAsync(appointment);
        }

        

        public async Task ComputeAppointmentService(Appointment app, AppointmentDisplay appD)
        {
            var listAdd = new List<ProductAppointmentRel>();
            var listRemove = new List<ProductAppointmentRel>();
            if (app.AppointmentServices.Any())
            {
                foreach (var item in app.AppointmentServices)
                {
                    if (!appD.Services.Any(x => x.Id == item.ProductId))
                    {
                        listRemove.Add(item);
                    }
                }
            }

            if (appD.Services.Any())
            {
                foreach (var item in appD.Services)
                {
                    if (!app.AppointmentServices.Any(x => x.ProductId == item.Id))
                    {
                        var prodApp = new ProductAppointmentRel()
                        {
                            AppoinmentId = app.Id,
                            ProductId = item.Id
                        };
                        listAdd.Add(prodApp);
                    }
                }
            }
            await _productAppointmentRelService.CreateAsync(listAdd);
            await _productAppointmentRelService.DeleteAsync(listRemove);
        }

        public void ComputeDataExcel(ExcelWorksheet worksheet, IEnumerable<AppointmentBasic> data, Dictionary<string, string> stateDict)
        {
            worksheet.Cells[1, 1].Value = "Khách hàng";
            worksheet.Cells[1, 2].Value = "Thời gian hẹn";
            worksheet.Cells[1, 3].Value = "Dịch vụ";
            worksheet.Cells[1, 4].Value = "Bác sĩ";
            worksheet.Cells[1, 5].Value = "Trạng thái";
            worksheet.Cells[1, 6].Value = "Lý do hủy hẹn";
            worksheet.Cells[1, 7].Value = "Nhãn khách hàng";
            worksheet.Cells[1, 8].Value = "SĐT";
            worksheet.Cells[1, 9].Value = "Tuổi";
            worksheet.Cells[1, 10].Value = "Nội dung";

            worksheet.Cells["A1:P1"].Style.Font.Bold = true;

            var row = 2;
            foreach (var item in data)
            {
                worksheet.Cells[row, 1].Value = item.PartnerDisplayName;
                worksheet.Cells[row, 2].Value = (item.Date.HasValue ? item.Date.Value.ToString("dd/MM/yyyy") + ", " : "") + item.Time;
                worksheet.Cells[row, 3].Value = string.Join(", ", item.Services.Select(x => x.Name));
                worksheet.Cells[row, 4].Value = item.DoctorName;
                worksheet.Cells[row, 5].Value = !string.IsNullOrEmpty(item.State) && stateDict.ContainsKey(item.State) ? stateDict[item.State] : "";
                worksheet.Cells[row, 6].Value = item.Reason;
                worksheet.Cells[row, 7].Value = string.Join(", ", item.Partner.Categories.OrderBy(x => x.Name).Select(x => x.Name));
                worksheet.Cells[row, 8].Value = item.PartnerPhone;
                worksheet.Cells[row, 9].Value = item.Partner.Age;
                worksheet.Cells[row, 10].Value = item.Note;

                row++;
            }
            worksheet.Column(4).Style.Numberformat.Format = "@";
            worksheet.Cells.AutoFitColumns();
        }

        public async Task<IEnumerable<EmployeeSimple>> GetListDoctor(AppointmentDoctorReq val)
        {
            var query = GetSearchQuery(dateFrom: val.DateFrom, dateTo: val.DateTo);
            var res = await query.Where(x=> x.DoctorId.HasValue).Distinct().Select(x => new EmployeeSimple() { Id = x.DoctorId.Value, Name = x.Doctor.Name }).ToListAsync();
            return res;
        }
    }
}
