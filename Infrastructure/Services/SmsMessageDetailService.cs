using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using ApplicationCore.Models;
using ApplicationCore.Specifications;
using AutoMapper;
using Hangfire;
using Infrastructure.Data;
using Infrastructure.HangfireJobService;
using Infrastructure.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MyERP.Utilities;
using Newtonsoft.Json;
using RestSharp;
using SaasKit.Multitenancy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Web.Models.ContentEditing;

namespace Infrastructure.Services
{
    public class SmsMessageDetailService : ISmsMessageDetailService
    {
        private readonly IMapper _mapper;
        private readonly AppTenant _tenant;
        private readonly IAsyncRepository<SmsMessageDetail> _repository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly CatalogDbContext _context;
        private readonly string SpecialCharactors = "@,_,{,},[,],|,~,\\,\\,$";
        private readonly string SpecialString = "khuyen mai,uu dai,tang,chiet khau,co hoi nhan ngay,co hoi boc tham,rut tham,trung thuong,giam***%,sale***upto,mua* tang*,giamgia,giam d," +
            "giamd,giam toi,giam den,giam gia,giam ngay,giam hoc phi,giam hphi,sale off,sale,sale d,kmai,giam-gia,uu-dai,sale-off,K.Mai,ma KM,hoc bong (.*) khi dang ky";
        public SmsMessageDetailService(
            IHttpContextAccessor httpContextAccessor,
            ITenant<AppTenant> tenant,
            IAsyncRepository<SmsMessageDetail> repository,
            CatalogDbContext context,
            IMapper mapper)
        {
            _mapper = mapper;
            _tenant = tenant?.Value;
            _context = context;
            _repository = repository;
            _httpContextAccessor = httpContextAccessor;
        }

        public IQueryable<SmsMessageDetail> GetQueryable(SmsMessageDetailPaged val)
        {
            var query = _repository.SearchQuery();
            if (val.PartnerId.HasValue)
                query = query.Where(x => x.PartnerId == val.PartnerId.Value);
            if (!string.IsNullOrEmpty(val.Search))
                query = query.Where(x => x.Number.Contains(val.Search) ||
                (x.PartnerId.HasValue && (x.Partner.Name.Contains(val.Search) || (x.Partner.NameNoSign.Contains(val.Search)))) ||
                (x.SmsMessageId.HasValue && x.SmsMessage.Name.Contains(val.Search)));
            if (!string.IsNullOrEmpty(val.State))
                query = query.Where(x => x.State.Equals(val.State));
            if (val.SmsCampaignId.HasValue)
                query = query.Where(x => x.SmsCampaignId.HasValue && x.SmsCampaignId.Value == val.SmsCampaignId.Value);
            if (val.DateFrom.HasValue)
                query = query.Where(x => x.DateCreated.HasValue && val.DateFrom.Value <= x.DateCreated.Value);
            if (val.DateTo.HasValue)
                query = query.Where(x => x.DateCreated.HasValue && val.DateTo.Value >= x.DateCreated.Value);
            if (val.SmsMessageId.HasValue)
                query = query.Where(x => x.SmsMessageId.HasValue && x.SmsMessageId.Value == val.SmsMessageId.Value);
            return query;
        }

        public async Task<PagedResult2<SmsMessageDetailStatistic>> GetPagedStatistic(SmsMessageDetailPaged val)
        {
            var query = GetQueryable(val);
            var totalItems = await query.CountAsync();
            var items = await query.Skip(val.Offset).Take(val.Limit).Select(x => new SmsMessageDetailStatistic
            {
                Id = x.Id,
                Body = x.Body,
                BrandName = x.SmsAccount.BrandName,
                DateCreated = x.DateCreated,
                ErrorCode = x.ErrorCode,
                Number = x.Number,
                PartnerName = x.Partner.DisplayName,
                SmsCampaignName = x.SmsCampaignId.HasValue ? x.SmsCampaign.Name : "",
                SmsMessageName = x.SmsMessageId.HasValue ? x.SmsMessage.Name : "",
                State = x.State
            }).ToListAsync();

            return new PagedResult2<SmsMessageDetailStatistic>(totalItems: totalItems, offset: val.Offset, limit: val.Limit)
            {
                Items = items
            };
        }

        public async Task<PagedResult2<SmsMessageDetailBasic>> GetPaged(SmsMessageDetailPaged val)
        {
            var query = GetQueryable(val);
            var totalItems = await query.CountAsync();
            var items = await query.Include(x => x.SmsAccount).Include(x => x.Partner).OrderByDescending(x => x.DateCreated).Skip(val.Offset).Take(val.Limit).ToListAsync();
            return new PagedResult2<SmsMessageDetailBasic>(totalItems: totalItems, limit: val.Limit, offset: val.Offset)
            {
                Items = _mapper.Map<IEnumerable<SmsMessageDetailBasic>>(items)
            };
        }

        public async Task ReSendSms(IEnumerable<SmsMessageDetail> details)
        {
            var smsAccountObj = GetService<ISmsAccountService>();
            var dictSmsAccount = await smsAccountObj.SearchQuery().ToDictionaryAsync(x => x.Id, x => x.Id);
            var smsMessageDetailGroup = details.GroupBy(x => x.SmsAccountId).ToDictionary(x => x.Key, x => x.Select(s => s.Id).ToList());
            foreach (var key in smsMessageDetailGroup.Keys)
            {
                if (dictSmsAccount.ContainsKey(key))
                {
                    await SendSMS(smsMessageDetailGroup[key], dictSmsAccount[key]);
                }
            }

        }

        /// <summary>
        /// de tam
        /// </summary>
        /// <returns></returns>
        public async Task RunJobSendSms()
        {
            var hostName = _tenant != null ? _tenant.Hostname : "localhost";
            var jobId = $"{hostName}_Send_Sms_message_detail";
            RecurringJob.AddOrUpdate<ISmsMessageJobService>(jobId, x => x.RunJobFindSmsMessage(hostName, CompanyId), $"*/5 * * * *", TimeZoneInfo.Local);
        }

        #region Gui tin nhan cho toi tac

        public async Task CreateSmsMessageDetail(SmsMessage smsMessage, IEnumerable<Guid> ids, Guid companyId)
        {
            var listSms = new List<SmsMessageDetail>();
            var listSmsErrors = new List<SmsMessageDetail>();
            var dictApp = new Dictionary<Guid, Appointment>();
            var partners = await _context.Partners.Where(x => ids.Contains(x.Id)).Include(x => x.Title).ToListAsync();
            if (smsMessage.ResModel == "appointment")
            {
                var appIds = smsMessage.SmsMessageAppointmentRels.Select(x => x.AppointmentId).ToList();
                dictApp = await _context.Appointments.Where(x => appIds.Contains(x.Id)).Include(x => x.Doctor).ToDictionaryAsync(x => x.PartnerId, x => x);
            }
            var company = await _context.Companies.Where(x => x.Id == companyId).FirstOrDefaultAsync();
            var listCharactorSpecial = SpecialCharactors.Split(",");
            var listStringSpecial = SpecialString.Split(",");
            foreach (var partner in partners)
            {
                var content = await PersonalizedContent(smsMessage.Body, partner, company, dictApp);
                var sms = new SmsMessageDetail();
                sms.Id = GuidComb.GenerateComb();
                sms.Body = content;
                sms.Number = !string.IsNullOrEmpty(partner.Phone) ? partner.Phone : "";
                sms.PartnerId = partner.Id;
                sms.State = "sending";
                sms.SmsAccountId = smsMessage.SmsAccountId.Value;
                sms.SmsMessageId = smsMessage.Id;
                sms.SmsCampaignId = smsMessage.SmsCampaignId;

                if (listStringSpecial.Any(x => content.Contains(x)) || listCharactorSpecial.Any(x => content.Contains(x)))
                {
                    sms.State = "fails";
                    sms.ErrorCode = "199"; ///199 chua ky tu dac biet
                    listSmsErrors.Add(sms);
                }
                else
                    listSms.Add(sms);
            }

            smsMessage.State = "success";
            _context.Entry(smsMessage).State = EntityState.Modified;
            await _context.SmsMessageDetails.AddRangeAsync(listSms);
            await _context.SmsMessageDetails.AddRangeAsync(listSmsErrors);
            await _context.SaveChangesAsync();

            var smsIds = listSms.Select(x => x.Id).ToList();

            await SendSMS(smsIds, smsMessage.SmsAccountId.Value);

        }

        private async Task<string> PersonalizedContent(string body, Partner partner, Company company, Dictionary<Guid, Appointment> dictApp)
        {
            var content = JsonConvert.DeserializeObject<Body>(body);
            var messageContent = content.text
                .Replace("{ten_khach_hang}", partner.Name.Split(' ').Last())
                .Replace("{ho_ten_khach_hang}", partner.DisplayName.Split(' ').Last())
                .Replace("{ten_cong_ty}", company.Name.Split(' ').Last())
                .Replace("{ngay_sinh}", partner.GetDateOfBirth().Split(' ').Last())
                .Replace("{danh_xung}", partner.Title != null ? partner.Title.Name.Split(' ').Last() : "")

                .Replace("{gio_hen}", dictApp.ContainsKey(partner.Id) ? dictApp[partner.Id].Time.Split(' ').Last() : "")
                .Replace("{ngay_hen}", dictApp.ContainsKey(partner.Id) ? dictApp[partner.Id].Date.ToString("dd/MM/yyyy").Split(' ').Last() : "")
                .Replace("{bac_si_lich_hen}", dictApp.ContainsKey(partner.Id) && dictApp[partner.Id].DoctorId.HasValue ? dictApp[partner.Id].Doctor.Name.Split(' ').Last() : "")
                .Replace("{bac_si_chi_tiet_dieu_tri}", partner.Name)
                .Replace("{so_phieu_dieu_tri}", partner.Name)
                .Replace("{dich_vu}", partner.Name);
            return messageContent;
        }

        public async Task SendSMS(List<Guid> lineIds, Guid accountId)
        {
            var lines = await _context.SmsMessageDetails.Where(x => lineIds.Contains(x.Id)).Include(x => x.Partner).ToListAsync();
            var account = await _context.SmsAccounts.Where(x => x.Id == accountId).FirstOrDefaultAsync();
            if (account.Provider == "fpt")
            {
                var modelToken = await FptGetAccessToken(account);
                var requestFpt = new List<FPTSendSMSRequestModel>();
                foreach (var line in lines)
                {
                    var fpt = new FPTSendSMSRequestModel();
                    fpt.Phone = line.Partner.Phone;
                    fpt.access_token = modelToken.access_token;
                    fpt.BrandName = account.BrandName;
                    fpt.Message = line.Body;
                    fpt.session_id = "789dC48b88e54f58ece5939f14a";
                    requestFpt.Add(fpt);
                }
                var responses = await FptSendSMS(requestFpt);
                if (responses != null && responses.Any())
                {
                    foreach (var res in responses)
                    {
                        if (res.SmsId.HasValue)
                        {
                            //dict.Add(res.SmsId.Value, res);
                        }
                    }
                }
            }
            else if (account.Provider == "esms")
            {
                foreach (var line in lines)
                {
                    var esms = new ESMSSendMessageRequestModel();
                    esms.Phone = line.Number;
                    esms.ApiKey = account.ApiKey;
                    esms.SecretKey = account.Secretkey;
                    esms.Brandname = account.BrandName;
                    esms.Content = line.Body;
                    esms.SmsType = "2";
                    esms.IsUnicode = 1;
                    var res = await ESmsSendSMS(esms);
                    if (res != null)
                    {
                        line.ErrorCode = res.CodeResult;
                        line.State = res.Message;
                    }
                    _context.Entry(line).State = EntityState.Modified;
                }
                await _context.SaveChangesAsync();
            }

        }

        public async Task<ESMSSendMessageResponseModel> ESmsSendSMS(ESMSSendMessageRequestModel val)
        {
            var model = new ESMSSendMessageResponseModel();
            var url = "http://rest.esms.vn/MainService.svc/json/SendMultipleMessage_V4_post_json/";
            var client = new HttpClient();
            var jsonObject = JsonConvert.SerializeObject(val);
            var content = new StringContent(jsonObject, Encoding.UTF8, "application/json");
            var result = await client.PostAsync(url, content);
            if (result.IsSuccessStatusCode)
            {
                var response = await result.Content.ReadAsStringAsync();
                model = JsonConvert.DeserializeObject<ESMSSendMessageResponseModel>(response);

                if (model != null && model.CodeResult.Equals("100"))
                {
                    model.Message = "success";
                }
                else
                {
                    model.Message = "fails";
                }
            }
            else
            {
                model.Message = "fails";
            }
            return model;
        }

        public async Task<FPTAccessTokenResponseModel> FptGetAccessToken(SmsAccount account)
        {
            var value = new FPTAccessTokenRequestModel();
            value.grant_type = "";
            value.client_id = account.ClientId;
            value.client_secret = account.ClientSecret;
            value.scope = "send_brandname_otp";
            value.session_id = "789dC48b88e54f58ece5939f14a";
            var url = "http://app.sms.fpt.net/oauth2/token";
            var model = new FPTAccessTokenResponseModel();
            var restRequest = new RestRequest().AddJsonBody(value);
            var client = new RestClient(url);
            var response = await client.ExecutePostAsync<FPTAccessTokenResponseModel>(restRequest);
            if (response.IsSuccessful)
            {
                model = response.Data;
            }
            return model;
        }

        public async Task<IEnumerable<FPTSendSMSResponseModel>> FptSendSMS(IEnumerable<FPTSendSMSRequestModel> vals)
        {
            var listReponse = new List<FPTSendSMSResponseModel>();
            foreach (var val in vals)
            {
                var url = "http://app.sms.fpt.net/api/push-brandname-otp";
                var model = new FPTSendSMSResponseModel();
                val.Message = Base64Encode(val.Message);
                var restRequest = new RestRequest().AddJsonBody(val);
                var client = new RestClient(url);
                var response = await client.ExecutePostAsync<FPTSendSMSResponseModel>(restRequest);
                if (response.IsSuccessful)
                {
                    model = response.Data;
                }
                listReponse.Add(model);
            }
            return listReponse;
        }

        public async Task<IEnumerable<VietguysSendMessageResponse>> VietguysSendSMS(IEnumerable<VietguysSendMessageRequest> vals)
        {
            var listReponse = new List<VietguysSendMessageResponse>();
            foreach (var val in vals.ToList())
            {
                var model = new VietguysSendMessageResponse();
                var url = "https://cloudsms.vietguys.biz:4438/api/index.php";
                var restClient = new RestClient(url);
                var resRequest = new RestRequest().AddJsonBody(val);
                var response = await restClient.ExecutePostAsync<VietguysSendMessageResponse>(resRequest);
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    model = JsonConvert.DeserializeObject<VietguysSendMessageResponse>(response.Content);
                    model.SmsId = val.LineId;
                    model.Message = model.error == 0 ? "success" : "fails";
                }
                else
                {
                    model.SmsId = val.LineId;
                    model.Message = "error";
                }
                listReponse.Add(model);
            }
            return listReponse;
        }

        public static string Base64Encode(string message)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(message);
            return System.Convert.ToBase64String(plainTextBytes);
        }

        public class Body
        {
            public string templateType { get; set; }
            public string text { get; set; }
        }

        public class ESMSSendMessageRequestModel
        {
            public Guid SmsId { get; set; }
            public string Phone { get; set; }
            public string Content { get; set; }
            public string ApiKey { get; set; }
            public string SecretKey { get; set; }
            public string SmsType { get; set; }
            public string Brandname { get; set; }
            public int IsUnicode { get; set; }
            public string Sandbox { get; set; }
        }

        public class ESMSSendMessageResponseModel
        {
            public string CodeResult { get; set; }
            public string CountRegenerate { get; set; }
            public string SMSID { get; set; }
            public string Message { get; set; }
        }

        public class FPTSendSMSRequestModel
        {
            /// <summary>
            /// Access token 
            /// </summary>
            public string access_token { get; set; }

            /// <summary>
            /// Session id lúc đăng ký 
            /// access token
            /// </summary>
            public string session_id { get; set; }

            /// <summary>
            /// Brandname đã đăng 
            /// ký với FPT
            /// </summary>
            public string BrandName { get; set; }

            /// <summary>
            /// Số điện thoại nhận tin 
            /// nhắn.Định dạng 
            /// 84xxx, 0xxx.
            /// Ví dụ: 84949123456 
            /// hoặc 0949123456
            /// </summary>
            public string Phone { get; set; }

            /// <summary>
            /// Nội dung tin nhắn gửi đi.
            /// Lưu ý: nội dung phải
            /// được mã hóa base64
            /// </summary>
            public string Message { get; set; }
        }

        public class FPTSendSMSResponseModel
        {
            /// <summary>
            /// Id của tin brandname gửi đi. 
            /// </summary>
            public int MessageId { get; set; }

            /// <summary>
            /// Tên brandname 
            /// </summary>
            public string BrandName { get; set; }

            /// <summary>
            /// Số điện thoại gửi tin nhắn. Định dạng 84xxxx
            /// </summary>
            public string Phone { get; set; }

            /// <summary>
            /// Nội dung tin nhắn gửi đi
            /// </summary>
            public string Message { get; set; }

            /// <summary>
            /// ID của đối tác
            /// </summary>
            public string PartnerId { get; set; }

            /// <summary>
            /// Nhà mạng của thuê bao khách hàng.
            /// </summary>
            public string Telco { get; set; }
            public Guid? SmsId { get; internal set; }
        }

        public class FPTAccessTokenRequestModel
        {
            public string grant_type { get; set; }
            public string client_id { get; set; }
            public string client_secret { get; set; }
            public string scope { get; set; }
            public string session_id { get; set; }
            public string brand_name { get; set; }
        }

        public class FPTAccessTokenResponseModel
        {
            public string access_token { get; set; }
            public int? expires_in { get; set; }
            public string token_type { get; set; }
            public string scope { get; set; }
            public int? error { get; set; }
            public string error_description { get; set; }
        }

        public class VietguysSendMessageRequest
        {
            public string u { get; set; }
            public string pwd { get; set; }
            public string from { get; set; }
            public string phone { get; set; }
            public string sms { get; set; }
            public string bid { get; set; }
            public int type { get; set; }
            public string json { get; set; }
            public Guid LineId { get; set; }
        }

        public class VietguysSendMessageResponse
        {
            public string msgid { get; set; }
            public int error { get; set; }
            public string log { get; set; }
            public string carrier { get; set; }
            public string error_code { get; set; }
            public Guid? SmsId { get; set; }
            public string Message { get; set; }
        }

        #endregion

        protected T GetService<T>()
        {
            return (T)_httpContextAccessor.HttpContext.RequestServices.GetService(typeof(T));
        }

        public IQueryable<SmsMessageDetail> SearchQuery()
        {
            var query = _repository.SearchQuery();
            return query;
        }

        public async Task<IEnumerable<ReportTotalOutputItem>> GetReportTotal(ReportTotalInput val)
        {
            var query = SearchQuery();

            if (val.Date.HasValue)
                query = query.Where(x => x.DateCreated.Value.Month == val.Date.Value.Month);

            var res = await query.GroupBy(x => new { x.State })
                .Select(x => new ReportTotalOutputItem
                {
                    State = x.Key.State,
                    Total = x.Count(),
                    Percentage = x.Count() * 100f / query.Count()
                }).ToListAsync();
            return res;
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
    }
}
