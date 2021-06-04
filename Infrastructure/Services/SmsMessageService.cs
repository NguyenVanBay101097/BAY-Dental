using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using ApplicationCore.Models;
using ApplicationCore.Specifications;
using AutoMapper;
using Infrastructure.Data;
using Infrastructure.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using SaasKit.Multitenancy;
using System;
using System.Collections.Generic;
using System.Linq;
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

        private readonly string SpecialCharactors = "@,_,{,},[,],|,~,\\,\\,$";
        private readonly string BacklistString = "khuyen mai,uu dai,tang,chiet khau,co hoi nhan ngay,co hoi boc tham,rut tham,trung thuong,giam***%,sale***upto,mua* tang*,giamgia,giam d," +
            "giamd,giam toi,giam den,giam gia,giam ngay,giam hoc phi,giam hphi,sale off,sale,sale d,kmai,giam-gia,uu-dai,sale-off,K.Mai,ma KM,hoc bong (.*) khi dang ky";

        public SmsMessageService(CatalogDbContext context,
            IMapper mapper,
            IAsyncRepository<SmsMessage> repository,
            IAsyncRepository<Partner> partnerRepository,
            IAsyncRepository<SmsMessageDetail> messageDetailRepository,
            IAsyncRepository<Appointment> appointmentRepository)
        {
            _mapper = mapper;
            _context = context;
            _messageRepository = repository;
            _partnerRepository = partnerRepository;
            _messageDetailRepository = messageDetailRepository;
            _appointmentRepository = appointmentRepository;
        }

        private IQueryable<SmsMessage> GetQueryable(SmsMessagePaged val)
        {
            //var query = _messageRepository.SearchQuery(x => x.CompanyId == CompanyId);
            //if (!string.IsNullOrEmpty(val.Search))
            //    query = query.Where(x => x.Name.Contains(val.Search));
            //if (!string.IsNullOrEmpty(val.State))
            //    query = query.Where(x => x.State == val.State);
            //if (val.CampaignId.HasValue)
            //    query = query.Where(x => x.SmsCampaignId.Value == val.CampaignId.Value);
            //if (val.SmsAccountId.HasValue)
            //    query = query.Where(x => x.SmsAccountId.HasValue && x.SmsAccountId == val.SmsAccountId.Value);
            //if (val.DateFrom.HasValue)
            //    query = query.Where(x => x.DateCreated.HasValue && val.DateFrom.Value <= x.Date.Value);
            //if (val.DateTo.HasValue)
            //    query = query.Where(x => x.DateCreated.HasValue && val.DateTo.Value >= x.Date.Value);
            //return query;
            return null;
        }

        public async Task<PagedResult2<SmsMessageBasic>> GetPaged(SmsMessagePaged val)
        {
            var query = GetQueryable(val);
            var totalItems = await query.CountAsync();
            var items = await query.Skip(val.Offset).Take(val.Limit).Select(x => new SmsMessageBasic
            {
                Id = x.Id,
                Date = x.Date,
                DateCreated = x.DateCreated,
                Name = x.Name,
                CountPartner = x.SmsMessagePartnerRels.Count(),
                BrandName = x.SmsAccountId.HasValue ? x.SmsAccount.BrandName + $" ({x.SmsAccount.Name})" : "",
            }).ToListAsync();
            return new PagedResult2<SmsMessageBasic>(totalItems, val.Offset, val.Limit)
            {
                Items = items
            };
        }

        public async Task<SmsMessageDisplay> CreateAsync(SmsMessageSave val)
        {
            var entity = _mapper.Map<SmsMessage>(val);
            //entity.CompanyId = CompanyId;
            if (entity.TypeSend == "manual")
                entity.State = "sending";
            else
            {
                entity.State = "waiting";
            }

            if (val.IsCareAfterOrder.HasValue && val.IsCareAfterOrder.Value)
            {
                entity.ResModel = "sale-order-line";
                if (val.GuidIds.Any())
                {
                    foreach (var id in val.GuidIds)
                    {
                        entity.SmsMessageSaleOrderLineRels.Add(new SmsMessageSaleOrderLineRel()
                        {
                            SaleOrderLineId = id
                        });
                    }
                }
            }

            else if (val.IsThanksCustomer.HasValue && val.IsThanksCustomer.Value)
            {
                entity.ResModel = "sale-order";
                if (val.GuidIds.Any())
                {
                    foreach (var id in val.GuidIds)
                    {
                        entity.SmsMessageSaleOrderRels.Add(new SmsMessageSaleOrderRel()
                        {
                            SaleOrderId = id
                        });
                    }
                }
            }

            else if (val.IsAppointmentReminder.HasValue && val.IsAppointmentReminder.Value)
            {
                entity.ResModel = "appointment";
                if (val.GuidIds.Any())
                {
                    foreach (var id in val.GuidIds)
                    {
                        entity.SmsMessageAppointmentRels.Add(new SmsMessageAppointmentRel()
                        {
                            AppointmentId = id
                        });
                    }
                }
            }

            else
            {
                entity.ResModel = "partner";
                if (val.GuidIds.Any())
                {
                    foreach (var id in val.GuidIds)
                    {
                        entity.SmsMessagePartnerRels.Add(new SmsMessagePartnerRel()
                        {
                            PartnerId = id
                        });
                    }
                }
            }
            entity = await _messageRepository.InsertAsync(entity);
            return _mapper.Map<SmsMessageDisplay>(entity);
        }

        public async Task ActionSendSMSMessage(SmsMessage entity)
        {
            await ActionSend(entity);
            //var smsMessageDetailObj = GetService<ISmsMessageDetailService>();
            //var hostName = _tenant != null ? _tenant.Hostname : "localhost";
            //await using var context = DbContextHelper.GetCatalogDbContext(hostName, _configuration);
            //try
            //{
            //    var companyId = CompanyId;

            //    await smsMessageDetailObj.CreateSmsMessageDetailV2(entity, companyId);

            //    entity.State = "success";
            //    await UpdateAsync(entity);
            //}
            //catch (Exception ex)
            //{
            //    throw new Exception(ex.Message);
            //}
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
                };
            }
        }

        public async Task ActionSend(SmsMessage self)
        {
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
            else if (self.ResModel == "saleorderline")
            {
            }
            else if (self.ResModel == "partner")
            {
            }
            else if (self.ResModel == "sale-order")
            {
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
                    Number = partner.Phone,
                    SmsAccountId = self.SmsAccountId.Value,
                    SmsMessageId = self.Id,
                    SmsCampaignId = self.SmsCampaignId
                };
                details.Add(detail);
            }

            await _messageDetailRepository.InsertAsync(details);

            var account = self.SmsAccount;
            var senderService = new ESmsSenderService(account.BrandName, account.ApiKey, account.Secretkey);
            var backlists = BacklistString.Split(",");
            var specialChars = SpecialCharactors.Split(",");
            foreach (var detail in details)
            {
                //neu truong hop detail ma co noi dung trong blacklist thi chuyen sang state canceled
                if (backlists.Any(x => detail.Body.Contains(x)) || specialChars.Any(x => detail.Body.Contains(x)))
                {
                    detail.State = "canceled";
                    detail.ErrorCode = "sms_blacklist";
                }
                else
                {
                    var sendResult = await senderService.SendAsync(detail.Number, detail.Body);
                    if (sendResult != null)
                    {
                        if (sendResult.CodeResult == "100")
                            detail.State = "sent";
                        else
                        {
                            detail.State = "error";
                            detail.ErrorCode = ESmsErrorCode[sendResult.CodeResult];
                        }
                    }
                    else
                    {
                        detail.State = "error";
                        detail.ErrorCode = "sms_server";
                    }

                }
            }

            await _messageDetailRepository.UpdateAsync(details);
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
                    .Replace("{bac_si_lich_hen}", app.Doctor != null ? app.Doctor.Name.Split(' ').Last() : "");

                dict.Add(app.Id, content);
            }

            return dict;
        }

        public async Task SendSMSDetails(List<SmsMessageDetail> details)
        {
            //var account = await _context.SmsAccounts.Where(x => x.Id == accountId).FirstOrDefaultAsync();
            //if (account.Provider == "fpt")
            //{
            //    var modelToken = await FptGetAccessToken(account);
            //    var requestFpt = new List<FPTSendSMSRequestModel>();
            //    foreach (var line in lines)
            //    {
            //        var fpt = new FPTSendSMSRequestModel();
            //        fpt.Phone = line.Partner.Phone;
            //        fpt.access_token = modelToken.access_token;
            //        fpt.BrandName = account.BrandName;
            //        fpt.Message = line.Body;
            //        fpt.session_id = "789dC48b88e54f58ece5939f14a";
            //        requestFpt.Add(fpt);
            //    }
            //    var responses = await FptSendSMS(requestFpt);
            //    if (responses != null && responses.Any())
            //    {
            //        foreach (var res in responses)
            //        {
            //            if (res.SmsId.HasValue)
            //            {
            //                //dict.Add(res.SmsId.Value, res);
            //            }
            //        }
            //    }
            //}
            //else if (account.Provider == "esms")
            //{
            //    foreach (var line in lines)
            //    {
            //        var esms = new ESMSSendMessageRequestModel();
            //        esms.Phone = line.Number;
            //        esms.ApiKey = account.ApiKey;
            //        esms.SecretKey = account.Secretkey;
            //        esms.Brandname = account.BrandName;
            //        esms.Content = line.Body;
            //        esms.SmsType = "2";
            //        esms.IsUnicode = 1;
            //        var res = await ESmsSendSMS(esms);
            //        if (res != null)
            //        {
            //            line.ErrorCode = res.CodeResult;
            //            line.State = res.Message;
            //        }
            //        _context.Entry(line).State = EntityState.Modified;
            //    }
            //    await _context.SaveChangesAsync();
            //}

        }


        public IDictionary<Guid, string> XuLyNoiDung(string template, IEnumerable<Partner> partners)
        {
            var dict = new Dictionary<Guid, string>();
            foreach (var partner in partners)
            {
                var content = template
                .Replace("{ten_khach_hang}", partner.Name.Split(' ').Last())
                .Replace("{ho_ten_khach_hang}", partner.DisplayName.Split(' ').Last())
                .Replace("{ngay_sinh}", partner.GetDateOfBirth().Split(' ').Last())
                .Replace("{danh_xung}", partner.Title != null ? partner.Title.Name.Split(' ').Last() : "");

                dict.Add(partner.Id, content);
            }

            return dict;
        }


        public async Task ActionCancel(IEnumerable<Guid> messIds)
        {
            //var messes = await SearchQuery(x => messIds.Contains(x.Id) && x.CompanyId == CompanyId).ToListAsync();
            //if (messes.Any())
            //{
            //    await DeleteAsync(messes);
            //}
        }

        public async Task SetupSendSmsOrderAutomatic(Guid orderId)
        {
            //var configObj = GetService<ISmsConfigService>();
            //var saleOrderObj = GetService<ISaleOrderService>();
            //var saleOrder = await saleOrderObj.SearchQuery(x => x.Id == orderId).FirstOrDefaultAsync();
            //var config = await configObj.SearchQuery(x => x.Type == "sale-order").FirstOrDefaultAsync();
            //if (config != null && config.IsThanksCustomerAutomation && saleOrder.State == "done" && saleOrder.DateDone.HasValue)
            //{
            //    var entity = new SmsMessage();
            //    var smsCampaignObj = GetService<ISmsCampaignService>();
            //    var campaign = await smsCampaignObj.GetDefaultThanksCustomer();
            //    entity.SmsCampaignId = campaign.Id;
            //    entity.ResModel = "sale-order";
            //    entity.Body = config.Body;
            //    entity.Date = config.TypeTimeBeforSend == "hour" ? saleOrder.DateDone.Value.AddHours(config.TimeBeforSend) : saleOrder.DateDone.Value.AddDays(config.TimeBeforSend);
            //    entity.SmsAccountId = config.SmsAccountId;
            //    entity.SmsTemplateId = config.TemplateId;
            //    entity.State = "waiting";
            //    entity.TypeSend = "automatic";
            //    entity.Name = $"Tin nhắn cảm ơn khách hàng ngày {DateTime.Now.ToString("dd-MM-yyyy HH:mm")}";
            //    entity.SmsMessageSaleOrderRels.Add(new SmsMessageSaleOrderRel()
            //    {
            //        SaleOrderId = saleOrder.Id
            //    });
            //    entity.CompanyId = CompanyId;

            //    await CreateAsync(entity);
            //}

        }

        public IQueryable<SmsMessage> SearchQuery()
        {

            return _messageRepository.SearchQuery();
        }

        
    }
}
