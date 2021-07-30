using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using ApplicationCore.Models;
using ApplicationCore.Specifications;
using ApplicationCore.Utilities;
using AutoMapper;
using Infrastructure.Data;
using Infrastructure.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using RestSharp;
using SaasKit.Multitenancy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Web.Models.ContentEditing;

namespace Infrastructure.Services
{
    public class SmsMessageService : ISmsMessageService
    {
        private readonly IMapper _mapper;
        private readonly CatalogDbContext _context;
        private readonly AppTenant _tenant;
        private readonly IAsyncRepository<Partner> _partnerRepository;
        private readonly IAsyncRepository<SmsMessage> _messageRepository;
        private readonly IAsyncRepository<SmsMessageDetail> _messageDetailRepository;
        private readonly IAsyncRepository<Appointment> _appointmentRepository;
        private readonly IAsyncRepository<SaleOrderLine> _saleLineRepository;
        private readonly IAsyncRepository<SaleOrder> _saleOrderRepository;
        private readonly IAsyncRepository<SmsCampaign> _smsCampaignRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IHttpClientFactory _clientFactory;
        private readonly IDistributedCache _cache;

        private readonly string SpecialCharactors = "@,_,{,},[,],|,~,\\,\\,$";
        private readonly string BacklistString = "khuyen mai,uu dai,tang,chiet khau,co hoi nhan ngay,co hoi boc tham,rut tham,trung thuong,giam***%,sale***upto,mua* tang*,giamgia,giam d," +
            "giamd,giam toi,giam den,giam gia,giam ngay,giam hoc phi,giam hphi,sale off,sale,sale d,kmai,giam-gia,uu-dai,sale-off,K.Mai,ma KM,hoc bong (.*) khi dang ky";

        public SmsMessageService(CatalogDbContext context,
            IMapper mapper,
            IHttpContextAccessor httpContextAccessor,
            IHttpClientFactory clientFactory,
            IDistributedCache cache,
            IAsyncRepository<SaleOrderLine> saleLineRepository,
            IAsyncRepository<SaleOrder> saleOrderRepository,
            IAsyncRepository<SmsMessage> repository,
            IAsyncRepository<Partner> partnerRepository,
            IAsyncRepository<SmsMessageDetail> messageDetailRepository,
            IAsyncRepository<Appointment> appointmentRepository,
            IAsyncRepository<SmsCampaign> smsCampaignRepository         
            )
        {
            _mapper = mapper;
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _messageRepository = repository;
            _saleOrderRepository = saleOrderRepository;
            _saleLineRepository = saleLineRepository;
            _partnerRepository = partnerRepository;
            _messageDetailRepository = messageDetailRepository;
            _appointmentRepository = appointmentRepository;
            _smsCampaignRepository = smsCampaignRepository;
            _clientFactory = clientFactory;
            _cache = cache;
        }

        private IQueryable<SmsMessage> GetQueryable(SmsMessagePaged val)
        {
            var query = _messageRepository.SearchQuery(x => x.CompanyId == CompanyId);
            if (!string.IsNullOrEmpty(val.Search))
                query = query.Where(x => x.Name.Contains(val.Search));
            if (!string.IsNullOrEmpty(val.State))
                query = query.Where(x => x.State == val.State);
            if (val.CampaignId.HasValue)
                query = query.Where(x => x.SmsCampaignId.Value == val.CampaignId.Value);
            if (val.SmsAccountId.HasValue)
                query = query.Where(x => x.SmsAccountId.HasValue && x.SmsAccountId == val.SmsAccountId.Value);
            if (val.DateFrom.HasValue)
                query = query.Where(x => val.DateFrom.Value <= x.Date.Value);
            if (val.DateTo.HasValue)
                query = query.Where(x => val.DateTo.Value >= x.Date.Value);
            return query;
        }

        public async Task<PagedResult2<SmsMessageBasic>> GetPaged(SmsMessagePaged val)
        {
            var query = GetQueryable(val);
            var totalItems = await query.CountAsync();
            var items = await query.OrderByDescending(x => x.DateCreated).Skip(val.Offset).Take(val.Limit).Select(x => new SmsMessageBasic
            {
                Id = x.Id,
                Date = x.Date,
                ScheduleDate = x.ScheduleDate,
                Name = x.Name,
                ResCount = x.ResCount.HasValue ? x.ResCount.Value : 0,
                BrandName = x.SmsAccount != null ? x.SmsAccount.DisplayName : "",
                DateCreated = x.DateCreated
            }).ToListAsync();

            var ids = items.Select(x => x.Id).ToList();
            var statistics = await _messageDetailRepository.SearchQuery(x => x.SmsMessageId.HasValue && ids.Contains(x.SmsMessageId.Value))
                .GroupBy(x => x.SmsMessageId.Value)
                .Select(x => new
                {
                    MessageId = x.Key,
                    Total = x.Sum(s => 1),
                    TotalSent = x.Sum(s => s.State == "sent" ? 1 : 0),
                    TotalError = x.Sum(s => s.State == "error" ? 1 : 0),
                }).ToListAsync();
            var statistics_dict = statistics.ToDictionary(x => x.MessageId, x => x);

            foreach (var item in items)
            {
                if (!statistics_dict.ContainsKey(item.Id))
                    continue;
                var statistic = statistics_dict[item.Id];
                item.Total = statistic.Total;
                item.TotalSent = statistic.TotalSent;
                item.TotalError = statistic.TotalError;
            }

            return new PagedResult2<SmsMessageBasic>(totalItems, val.Offset, val.Limit)
            {
                Items = items
            };
        }

        public async Task<SmsMessageDisplay> CreateAsync(SmsMessageSave val)
        {
            var entity = ComputeLines(val);
            entity.ResCount = val.ResIds.Count();
            entity = await _messageRepository.InsertAsync(entity);
            return _mapper.Map<SmsMessageDisplay>(entity);
        }

        public SmsMessage ComputeLines(SmsMessageSave val)
        {
            var entity = _mapper.Map<SmsMessage>(val);
            entity.CompanyId = CompanyId;

            if (entity.ResModel == "sale-order-line")
            {
                foreach (var id in val.ResIds)
                {
                    entity.SmsMessageSaleOrderLineRels.Add(new SmsMessageSaleOrderLineRel()
                    {
                        SaleOrderLineId = id
                    });
                }
            }
            else if (entity.ResModel == "sale-order")
            {

                foreach (var id in val.ResIds)
                {
                    entity.SmsMessageSaleOrderRels.Add(new SmsMessageSaleOrderRel()
                    {
                        SaleOrderId = id
                    });
                }

            }
            else if (entity.ResModel == "appointment")
            {
                foreach (var id in val.ResIds)
                {
                    entity.SmsMessageAppointmentRels.Add(new SmsMessageAppointmentRel()
                    {
                        AppointmentId = id
                    });
                }
            }
            else if (entity.ResModel == "partner")
            {
                foreach (var id in val.ResIds)
                {
                    entity.SmsMessagePartnerRels.Add(new SmsMessagePartnerRel()
                    {
                        PartnerId = id
                    });
                }
            }
            return entity;
        }

        private IDictionary<string, string> ESmsErrorCode
        {
            get
            {
                return new Dictionary<string, string>()
                {
                    { "99", "sms_server" },
                    { "101", "sms_acc" },
                    { "102", "sms_acc" },
                    { "103", "sms_credit" },
                    { "104", "sms_acc" },
                    { "146", "sms_template" },
                };
            }
        }

        public async Task ActionSend(Guid id)
        {
            var self = await _messageRepository.GetByIdAsync(id);
            if (self == null)
                return;

            if (self.SmsCampaignId.HasValue)
            {
                var campaign = await _smsCampaignRepository.SearchQuery(x => x.Id == self.SmsCampaignId).Include(x => x.MessageDetails).FirstOrDefaultAsync();
                if (campaign.LimitMessage > 0)
                {
                    var remain = Math.Max(campaign.LimitMessage - campaign.MessageDetails.Count, 0);
                    if ((self.ResCount ?? 0) > remain)
                        throw new Exception("Vượt hạn mức gửi tin cho phép");
                }
            }

            IDictionary<Guid, string> dict = null; //noi dung
            IEnumerable<object> objs = null;
            if (self.ResModel == "appointment")
            {
                _context.Entry(self).Collection(s => s.SmsMessageAppointmentRels).Load();
                var appointmentIds = self.SmsMessageAppointmentRels.Select(x => x.AppointmentId).ToList();
                objs = await _appointmentRepository.SearchQuery(x => appointmentIds.Contains(x.Id))
                    .Include(x => x.Partner)
                    .ToListAsync();

                dict = XuLyNoiDungLichHen(self.Body, (IEnumerable<Appointment>)objs);
            }
            else if (self.ResModel == "sale-order-line")
            {
                _context.Entry(self).Collection(s => s.SmsMessageSaleOrderLineRels).Load();
                var saleLinesIds = self.SmsMessageSaleOrderLineRels.Select(x => x.SaleOrderLineId).ToList();
                objs = await _saleLineRepository.SearchQuery(x => saleLinesIds.Contains(x.Id))
                    .Include(x => x.OrderPartner).ThenInclude(x => x.Title)
                    .Include(x => x.Order)
                    .Include(x => x.Employee)
                    .ToListAsync();

                dict = XuLyNoiDungChiTietDieuTri(self.Body, (IEnumerable<SaleOrderLine>)objs);
            }
            else if (self.ResModel == "partner")
            {
                _context.Entry(self).Collection(s => s.SmsMessagePartnerRels).Load();
                var partnerIds = self.SmsMessagePartnerRels.Select(x => x.PartnerId).ToList();
                objs = await _partnerRepository.SearchQuery(x => partnerIds.Contains(x.Id))
                    .Include(x => x.Title)
                    .ToListAsync();
                dict = XuLyNoiDungKhachHang(self.Body, (IEnumerable<Partner>)objs);
            }
            else if (self.ResModel == "sale-order")
            {
                _context.Entry(self).Collection(s => s.SmsMessageSaleOrderRels).Load();
                var saleOrderIds = self.SmsMessageSaleOrderRels.Select(x => x.SaleOrderId).ToList();
                objs = await _saleOrderRepository.SearchQuery(x => saleOrderIds.Contains(x.Id))
                    .Include(x => x.Partner).Select(x => x.Partner)
                    .ToListAsync();
                dict = XuLyNoiDungKhachHang(self.Body, (IEnumerable<Partner>)objs);
            }
            else
                throw new Exception("not support");

            var details = new List<SmsMessageDetail>();
            foreach (var item in objs)
            {
                var partner = GetPartnerFromObject(item);
                var itemId = Guid.Parse(item.GetType().GetProperty("Id").GetValue(item).ToString());
                var detail = new SmsMessageDetail
                {
                    Body = dict[itemId],
                    CompanyId = self.CompanyId,
                    PartnerId = partner.Id,
                    Date = DateTime.Now,
                    Number = partner.Phone,
                    SmsAccountId = self.SmsAccountId.Value,
                    SmsMessageId = self.Id,
                    SmsCampaignId = self.SmsCampaignId
                };
                details.Add(detail);
            }

            await _messageDetailRepository.InsertAsync(details);

            _context.Entry(self).Reference(s => s.SmsAccount).Load();
            var account = self.SmsAccount;

            await ActionSendSmsMessageDetail(details, account);

            self.State = "done";
            self.Date = DateTime.Now;
            await _messageRepository.UpdateAsync(self);

        }

        public async Task ActionSendSmsMessageDetail(IEnumerable<SmsMessageDetail> sefts, SmsAccount account)
        {

            var backlists = BacklistString.Split(",");
            var specialChars = SpecialCharactors.Split(",");

            //var senderService = new FptSenderService();
            //var senderService = new FptSenderService(_clientFactory, account.ClientId , account.ClientSecret);
            var senderService = new FptSmsService(_clientFactory.CreateClient());
            senderService.AuthConfig = new FptAuthConfig(account.ClientId, account.ClientSecret, new string[] { "send_brandname", "send_brandname_otp" });


            //Get accress token brand name send sms
            //var access_token = await senderService.GetApiToken(account.ClientId , account.ClientSecret);
            foreach (var detail in sefts)
            {
                //neu truong hop detail ma co noi dung trong blacklist thi chuyen sang state canceled
                if (backlists.Any(x => detail.Body.Contains(x)) || specialChars.Any(x => detail.Body.Contains(x)))
                {
                    detail.State = "canceled";
                    detail.ErrorCode = "sms_blacklist";
                }
                //else if (access_token.error.HasValue || access_token == null)
                //{
                //    detail.State = "error";
                //    detail.ErrorCode = access_token == null ? "sms_server" : access_token.error_description;
                //}
                else
                {
                    var sendResult = await senderService.SendSms(account.BrandName, detail.Number, detail.Body);
                    if (sendResult.Error == 0)
                        detail.State = "sent";
                    else
                    {
                        detail.State = "error";
                        detail.ErrorCode = sendResult.Error + "";
                    }
                }
            }

            await _messageDetailRepository.UpdateAsync(sefts);
        }    

        private Partner GetPartnerFromObject(object item)
        {
            if (item is Partner)
            {
                Partner partner = (Partner)item;
                return partner;
            }
            else if (item is SaleOrderLine)
            {
                SaleOrderLine saleLine = (SaleOrderLine)item;
                return saleLine.OrderPartner;
            }
            else if (item is SaleOrder)
            {
                SaleOrder saleOrder = (SaleOrder)item;
                return saleOrder.Partner;
            }
            else if (item is Appointment)
            {
                Appointment appointment = (Appointment)item;
                return appointment.Partner;
            }

            return null;
        }

        public IDictionary<Guid, string> XuLyNoiDungLichHen(string template, IEnumerable<Appointment> appointments)
        {
            var dict = new Dictionary<Guid, string>();
            foreach (var app in appointments)
            {
                var content = template
                    .Replace("{gio_hen}", app.Time.Split(' ').Last())
                    .Replace("{ngay_hen}", app.Date.ToString("dd/MM/yyyy").Split(' ').Last())
                    .Replace("{bac_si_lich_hen}", app.Doctor != null ? app.Doctor.Name.Split(' ').Last() : "")
                    .Replace("{ten_khach_hang}", app.Partner != null ? app.Partner.Name.Split(' ').Last() : "")
                    .Replace("{danh_xung}", app.Partner != null && app.Partner.Title != null ? app.Partner.Title.Name.Split(' ').Last() : "");

                dict.Add(app.Id, content);
            }

            return dict;
        }

        public IDictionary<Guid, string> XuLyNoiDungChiTietDieuTri(string template, IEnumerable<SaleOrderLine> lines)
        {
            var dict = new Dictionary<Guid, string>();
            foreach (var line in lines)
            {
                var content = template
                    .Replace("{bac_si}", line != null && line.Employee != null ? line.Employee.Name : "")
                    .Replace("{so_phieu_dieu_tri}", line != null && line.Order != null ? line.Order.Name : "")
                    .Replace("{dich_vu}", line != null ? line.Name : "")
                    .Replace("{ten_khach_hang}", line.OrderPartner != null ? line.OrderPartner.Name.Split(' ').Last() : "")
                    .Replace("{danh_xung}", line.OrderPartner != null && line.OrderPartner.Title != null ? line.OrderPartner.Title.Name.Split(' ').Last() : "");
                dict.Add(line.Id, content);
            }

            return dict;
        }

        public IDictionary<Guid, string> XuLyNoiDungKhachHang(string template, IEnumerable<Partner> partners)
        {
            var dict = new Dictionary<Guid, string>();
            foreach (var partner in partners)
            {
                var content = template
                .Replace("{ten_khach_hang}", partner.Name.Split(' ').Last())
                .Replace("{ngay_sinh}", partner.GetDateOfBirth().Split(' ').Last())
                .Replace("{danh_xung}", partner.Title != null ? partner.Title.Name.Split(' ').Last() : "");

                dict.Add(partner.Id, content);
            }

            return dict;
        }

        public async Task ActionCancel(IEnumerable<Guid> messIds)
        {
            var messes = await SearchQuery().Where(x => messIds.Contains(x.Id) && x.CompanyId == CompanyId).ToListAsync();
            if (messes.Any(x => x.State != "in_queue"))
                throw new Exception("Bạn chỉ có thể hủy tin nhắn chờ gửi");
            foreach (var item in messes)
                item.State = "cancelled";
            await _messageRepository.UpdateAsync(messes);
        }

        public async Task SetupSendSmsOrderAutomatic(Guid orderId)
        {
            var configObj = GetService<ISmsThanksCustomerAutomationConfigService>();
            var saleOrderObj = GetService<ISaleOrderService>();
            var saleOrder = await saleOrderObj.SearchQuery(x => x.Id == orderId).FirstOrDefaultAsync();
            var config = await configObj.SearchQuery().FirstOrDefaultAsync();
            if (config != null && config.Active && saleOrder.State == "done" && saleOrder.DateDone.HasValue)
            {
                var entity = new SmsMessage();
                var smsCampaignObj = GetService<ISmsCampaignService>();
                var campaign = await smsCampaignObj.GetDefaultThanksCustomer();
                entity.SmsCampaignId = campaign.Id;
                entity.ResModel = "sale-order";
                entity.Body = config.Body;
                entity.ScheduleDate = config.TypeTimeBeforSend == "hour" ? saleOrder.DateDone.Value.AddHours(config.TimeBeforSend) : saleOrder.DateDone.Value.AddDays(config.TimeBeforSend);
                entity.SmsAccountId = config.SmsAccountId;
                entity.SmsTemplateId = config.TemplateId;
                entity.CompanyId = config.CompanyId;
                entity.State = "in_queue";
                entity.Name = $"Tin nhắn cảm ơn khách hàng ngày {DateTime.Now.ToString("dd-MM-yyyy HH:mm")}";
                entity.SmsMessageSaleOrderRels.Add(new SmsMessageSaleOrderRel()
                {
                    SaleOrderId = saleOrder.Id
                });
                entity.CompanyId = CompanyId;

                await _messageRepository.InsertAsync(entity);
            }

        }

        public IQueryable<SmsMessage> SearchQuery()
        {

            return _messageRepository.SearchQuery();
        }

        protected T GetService<T>()
        {
            return (T)_httpContextAccessor.HttpContext.RequestServices.GetService(typeof(T));
        }

        public async Task UpdateAsync(SmsMessage entity)
        {
            await _messageRepository.UpdateAsync(entity);
        }

        public async Task DeleteAsync(SmsMessage entity)
        {
            await _messageRepository.DeleteAsync(entity);
        }

        public async Task<SmsMessage> GetByIdAsync(Guid id)
        {
            var entity = await _messageRepository.GetByIdAsync(id);
            return entity;
        }

        public async Task<SmsMessage> CreateAsync(SmsMessage entity)
        {
            entity = await _messageRepository.InsertAsync(entity);
            return entity;
        }

        public async Task<SmsMessageDisplay> ActionCreateReminder(SmsMessageSave val)
        {
            var entity = ComputeLines(val);
            entity.ResCount = val.ResIds.Count();
            entity.State = "in_queue";
            entity = await _messageRepository.InsertAsync(entity);
            return _mapper.Map<SmsMessageDisplay>(entity);
        }

        protected Guid CompanyId
        {
            get
            {
                if (!_httpContextAccessor.HttpContext.User.Identity.IsAuthenticated)
                    return Guid.Empty;
                var claim = _httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(x => x.Type == "company_id");
                return claim != null ? Guid.Parse(claim.Value) : Guid.Empty;
            }
        }
        //public override ISpecification<SmsMessage> RuleDomainGet(IRRule rule)
        //{
        //    switch (rule.Code)
        //    {
        //        case "sms.sms_message_comp_rule":
        //            return new InitialSpecification<SmsConfig>(x => !x.CompanyId.HasValue || x.CompanyId == CompanyId);
        //        default:
        //            return null;
        //    }
        //}
    }
}
