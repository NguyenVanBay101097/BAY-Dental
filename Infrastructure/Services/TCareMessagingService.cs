using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using ApplicationCore.Models;
using ApplicationCore.Specifications;
using ApplicationCore.Utilities;
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
    public class TCareMessagingService : BaseService<TCareMessaging>, ITCareMessagingService
    {
        private readonly IMapper _mapper;
        public TCareMessagingService(IAsyncRepository<TCareMessaging> repository, IHttpContextAccessor httpContextAccessor, IMapper mapper)
            : base(repository, httpContextAccessor)
        {
            _mapper = mapper;
        }

        public async Task<PagedResult2<TCareMessagingBasic>> GetPagedResultAsync(TCareMessagingPaged val)
        {
            ISpecification<TCareMessaging> spec = new InitialSpecification<TCareMessaging>(x => true);
           
            if (val.TCareScenarioId.HasValue)
                spec = spec.And(new InitialSpecification<TCareMessaging>(x => x.TCareCampaign.TCareScenarioId.Value == val.TCareScenarioId.Value));                
            if (val.DateFrom.HasValue)
            {
                var dateFrom = val.DateFrom.Value.AbsoluteBeginOfDate();
                spec = spec.And(new InitialSpecification<TCareMessaging>(x => x.ScheduleDate.Value >= dateFrom));             
            }
            if (val.DateTo.HasValue)
            {
                var dateTo = val.DateTo.Value.AbsoluteEndOfDate();
                spec = spec.And(new InitialSpecification<TCareMessaging>(x => x.ScheduleDate.Value <= dateTo));
               
            }

            var query = SearchQuery(spec.AsExpression(), orderBy: x => x.OrderByDescending(s => s.DateCreated));

            var items = await _mapper.ProjectTo<TCareMessagingBasic>(query.Skip(val.Offset).Take(val.Limit)).ToListAsync();
            var totalItems = await query.CountAsync();
            return new PagedResult2<TCareMessagingBasic>(totalItems, val.Offset, val.Limit)
            {
                Items = items
            };
        }

        public async Task<TCareMessagingDisplay> GetDisplay(Guid id)
        {
            var mes = await SearchQuery(x => x.Id == id)
                .Include(x => x.TCareCampaign)
                .Select(x => new TCareMessagingDisplay
                {
                    Id = x.Id,
                    //MethodType = x.MethodType,
                    //IntervalType = x.IntervalType,
                    //IntervalNumber = x.IntervalNumber,
                    //SheduleDate = x.SheduleDate,
                    Content = x.Content,
                    TCareCampaignId = x.TCareCampaignId,
                })
                .FirstOrDefaultAsync();
            if (mes == null)
                throw new Exception("Tin nhắn không tồn tại !!!");
            return mes;
        }

        public async Task<TCareMessaging> Create(TCareMessagingSave val) {
            var mes = _mapper.Map<TCareMessaging>(val);


            await CreateAsync(mes);

            return mes;

        }

        public async Task<TCareMessaging> Update(Guid id ,TCareMessagingSave val)
        {
          
            var mes = await SearchQuery(x => x.Id == id)
                .FirstOrDefaultAsync();

            if (mes == null)
                throw new Exception("Tin nhắn không tồn tại");
            if (val.ChannelType == null)
                throw new Exception("Vui lòng chọn phương thức kênh gửi");
            if (val.ChannelSocialId == null)
                throw new Exception("Vui lòng nhập kênh xã hội ");
       
                 
            mes = _mapper.Map(val, mes);
            await UpdateAsync(mes);
            return mes;

        }

       
    }
}
