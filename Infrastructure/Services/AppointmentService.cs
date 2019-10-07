using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using ApplicationCore.Models;
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
                .Include("Doctor")
                .FirstOrDefaultAsync();
            //Xác định cuộc hẹn đã có đợt khám tham chiếu hay chưa
            var dotKhamObj = GetService<IDotKhamService>();
            var query = await dotKhamObj.SearchQuery(x => x.AppointmentId == id).ToListAsync();
            var res = _mapper.Map<AppointmentDisplay>(category);
            if (query.Count()>0)
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
            //if (!string.IsNullOrEmpty(val.SearchByCustomer))
            //    query = query.Where(x => x.Partner.Name.Contains(val.SearchByCustomer) || x.Partner.Phone.Contains(val.SearchByCustomer) || x.Partner.Ref.Contains(val.SearchByCustomer));
            //if (!string.IsNullOrEmpty(val.SearchByDoctor))
            //    query = query.Where(x => x.User.Name.Contains(val.SearchByDoctor));

            if (val.DateTimeFrom.HasValue)
                query = query.Where(x => x.Date >= val.DateTimeFrom);
            if (val.DateTimeTo.HasValue)
                query = query.Where(x => x.Date <= val.DateTimeTo);

            if (val.DotKhamId.HasValue)
                query = query.Where(x => x.DotKhamId == val.DotKhamId);

            string[] stateList = null;
            if (!string.IsNullOrEmpty(val.State))
            {
                stateList = (val.State).Split(",");
                query = query.Where(x => stateList.Contains(x.State));
            }


            query = query.OrderByDescending(x => x.DateCreated);
            var items = new List<Appointment>();
            if (val.Limit > 0)
            {
                items = await query.Skip(val.Offset).Take(val.Limit)
                .Include(x => x.Partner).Include(x => x.User).Include(x=>x.Doctor)
                .ToListAsync();
            }
            else
            {
                items = await query
                    .Include(x => x.Partner).Include(x => x.User).Include(x => x.Doctor)
                .ToListAsync();
            }



            var totalItems = await query.CountAsync();

            return new PagedResult2<AppointmentBasic>(totalItems, val.Offset, val.Limit)
            {
                Items = _mapper.Map<IEnumerable<AppointmentBasic>>(items)
            };
        }

        public async Task<AppointmentDisplay> DefaultGet(AppointmentDefaultGet val)
        {
            var res = new AppointmentDisplay();
            res.CompanyId = CompanyId;
            res.UserId = UserId;
            var userManager = (UserManager<ApplicationUser>)_httpContextAccessor.HttpContext.RequestServices.GetService(typeof(UserManager<ApplicationUser>));
            var user = await userManager.FindByIdAsync(UserId);
            res.User = _mapper.Map<ApplicationUserSimple>(user);
            if (val.DotKhamId.HasValue)
            {
                var dkObj = GetService<IDotKhamService>();
                var dk = await dkObj.SearchQuery(x => x.Id == val.DotKhamId).Include(x => x.Partner).FirstOrDefaultAsync();
                res.DotKhamId = dk.Id;
                if (dk.PartnerId.HasValue)
                    res.PartnerId = dk.PartnerId.Value;
                if (dk.Partner != null)
                    res.Partner = _mapper.Map<PartnerSimple>(dk.Partner);
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
    }
}
