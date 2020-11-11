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

namespace Infrastructure.Services
{
    public class AppointmentService : BaseService<Appointment>, IAppointmentService
    {
        private readonly IMapper _mapper;
        public AppointmentService(IAsyncRepository<Appointment> repository, IHttpContextAccessor httpContextAccessor,
            IMapper mapper)
        : base(repository, httpContextAccessor)
        {
            _mapper = mapper;
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
            var query = SearchQuery();
            if (!string.IsNullOrEmpty(val.Search))
                query = query.Where(x => x.Name.Contains(val.Search) || x.Doctor.Name.Contains(val.Search)
                || x.Partner.Name.Contains(val.Search) || x.Partner.Phone.Contains(val.Search)
                || x.Partner.Ref.Contains(val.Search));

            if (val.DateTimeFrom.HasValue)
                query = query.Where(x => x.Date >= val.DateTimeFrom);

            if (val.DateTimeTo.HasValue)
            {
                var dateTo = val.DateTimeTo.Value.AbsoluteEndOfDate();
                query = query.Where(x => x.Date <= dateTo);
            }

            if (val.DotKhamId.HasValue)
                query = query.Where(x => x.DotKhamId == val.DotKhamId);

            if (!string.IsNullOrEmpty(val.UserId))
                query = query.Where(x => x.UserId == val.UserId);

            string[] stateList = null;
            if (!string.IsNullOrEmpty(val.State))
            {
                stateList = (val.State).Split(",");
                query = query.Where(x => stateList.Contains(x.State));
            }

            if (val.PartnerId.HasValue)
            {
                query = query.Where(x => x.PartnerId == val.PartnerId);
            }

            query = query.OrderByDescending(x => x.DateCreated);
            var limit = val.Limit > 0 ? val.Limit : int.MaxValue;
            var totalItems = await query.CountAsync();
            var items = await _mapper.ProjectTo<AppointmentBasic>(query.Skip(val.Offset).Take(limit)).ToListAsync();

            return new PagedResult2<AppointmentBasic>(totalItems, val.Offset, limit)
            {
                Items = items
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

            return res;
        }
        //Đếm số cuộc hẹn trong ngày (trang Tổng quan)
        public async Task<IEnumerable<AppointmentStateCount>> CountAppointment(DateFromTo val)
        {
            var today = DateTime.Today;
            var fromDate = val.DateFrom.HasValue ? val.DateFrom : val.DateFrom.HasValue ? val.DateFrom : today;
            var toDate = val.DateFrom.HasValue ? val.DateTo.Value.AddDays(1).AddMinutes(-1) : today.AddDays(1).AddMinutes(-1);

            var confirmCount = await SearchQuery().Where(x => x.Date > fromDate && x.Date < toDate && x.State.Contains("confirmed")).CountAsync();
            var cancelCount = await SearchQuery().Where(x => x.Date > fromDate && x.Date < toDate && x.State.Contains("cancel")).CountAsync();
            var doneCount = await SearchQuery().Where(x => x.Date > fromDate && x.Date < toDate && x.State.Contains("done")).CountAsync();
            var waitingCount = await SearchQuery().Where(x => x.Date > fromDate && x.Date < toDate && x.State.Contains("waiting")).CountAsync();
            var expiredCount = await SearchQuery().Where(x => x.Date > fromDate && x.Date < toDate && x.State.Contains("expired")).CountAsync();

            var list = new List<AppointmentStateCount>();
            list.Add(new AppointmentStateCount { State = "confirmed", Count = confirmCount, Color = "#04c835" });
            list.Add(new AppointmentStateCount { State = "cancel", Count = cancelCount, Color = "#cc0000" });
            list.Add(new AppointmentStateCount { State = "done", Count = doneCount, Color = "#666666" });
            list.Add(new AppointmentStateCount { State = "waiting", Count = waitingCount, Color = "#0080ff" });
            list.Add(new AppointmentStateCount { State = "expired", Count = expiredCount, Color = "#ffbf00" });

            return list;
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

            var items = await _mapper.ProjectTo<AppointmentBasic>(query.OrderBy(x => x.Date).Skip(val.Offset).Take(val.Limit)).ToListAsync();
            var count = await query.CountAsync();

            var res = new PagedResult2<AppointmentBasic>(count, val.Offset, val.Limit) { Items = items };
            return res;
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


    }
}
