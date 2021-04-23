using ApplicationCore.Entities;
using Infrastructure.Data;
using Infrastructure.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MyERP.Utilities;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Infrastructure.HangfireJobService
{
    public class SmsJobService : ISmsJobService
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public SmsJobService(IHttpContextAccessor httpContextAccessor, IConfiguration configuration)
        {
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task RunJob(string db, Guid configId)
        {
            await using var context = DbContextHelper.GetCatalogDbContext(db, _configuration);
            await using var transaction = await context.Database.BeginTransactionAsync();
            try
            {
                var smsConfig = await context.SmsConfigs.Include(x => x.BirthdayTemplate).Include(x => x.AppointmentTemplate).LastOrDefaultAsync();
                if (smsConfig.IsAppointmentAutomation)
                    await RunAppointmentAutomatic(context, smsConfig);
                else if (smsConfig.IsBirthdayAutomation)
                    await RunBirthdayAutomatic(context, smsConfig);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw ex;
            }
        }

        public async Task RunAppointmentAutomatic(CatalogDbContext context, SmsConfig config)
        {
            var now = DateTime.Now;
            var smsConfig = await context.SmsConfigs.LastOrDefaultAsync();
            var query = context.Appointments.AsQueryable();
            var listAppointments = await query.Where(x => !x.DateAppointmentReminder.HasValue &&
              x.DateTimeAppointment.HasValue
              && x.DateTimeAppointment.Value >= now
              && x.DateTimeAppointment.Value.AddHours(-1) <= now).ToListAsync();
            if (listAppointments.Any())
            {
                await CreateSmsComposer(context, listAppointments, config);
                foreach (var appointment in listAppointments)
                {
                    appointment.DateAppointmentReminder = now;
                }
                context.SaveChanges();
            }
        }

        public Task RunBirthdayAutomatic(CatalogDbContext context, SmsConfig config)
        {
            throw new NotImplementedException();
        }

        public async Task CreateSmsComposer(CatalogDbContext context, IEnumerable<Appointment> appointments, SmsConfig config)
        {
            var smsComposer = new SmsComposer();
            var partnerIds = appointments.Select(x => x.PartnerId);
            smsComposer.Id = GuidComb.GenerateComb();
            smsComposer.ResModel = "res.appointment";
            smsComposer.ResIds = string.Join(",", appointments.Select(x => x.Id));
            smsComposer.TemplateId = config.AppointmentTemplateId;
            smsComposer.Body = config.AppointmentTemplate.Body;
            context.SmsComposers.Add(smsComposer);
            await context.SaveChangesAsync();
            await CreateSmsSms(context, smsComposer, partnerIds);
        }

        public async Task CreateSmsSms(CatalogDbContext context, SmsComposer composer, IEnumerable<Guid> ids)
        {
            var listSms = new List<SmsSms>();
            var partners = await context.Partners.Where(x => ids.Contains(x.Id)).ToListAsync();
            foreach (var partner in partners)
            {
                var sms = new SmsSms();
                sms.Body = composer.Body;
                sms.Number = !string.IsNullOrEmpty(partner.Phone) ? partner.Phone : "";
                sms.PartnerId = partner.Id;
                sms.State = "sending";
                listSms.Add(sms);
            }
            var dict = await SendSMS(listSms, context);
            foreach (var item in listSms)
            {
                if (dict.ContainsKey(item.Id))
                {
                    item.State = dict[item.Id];
                }
            }

            await context.SmsSmss.AddRangeAsync(listSms);
            await context.SaveChangesAsync();

        }

        public async Task<Dictionary<Guid, string>> SendSMS(IEnumerable<SmsSms> lines, CatalogDbContext context)
        {
            var dict = new Dictionary<Guid, string>();
            var account = await context.SmsAccounts.LastOrDefaultAsync();
            if (account == null) throw new Exception("Bạn chưa cài đặt Brand Name");
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
                            dict.Add(res.SmsId.Value, res.Message);
                        }
                    }
                }
            }
            else if (account.Provider == "e-sms")
            {
                var requestEsms = new List<ESMSSendMessageRequestModel>();
                foreach (var line in lines)
                {
                    var esms = new ESMSSendMessageRequestModel();
                    esms.Phone = line.Partner.Phone;
                    esms.SmsId = line.Id;
                    esms.ApiKey = account.ApiKey;
                    esms.SecretKey = account.Secretkey;
                    esms.Brandname = account.BrandName;
                    esms.Content = line.Body;
                    esms.SmsType = "2";
                    esms.IsUnicode = 1;
                    requestEsms.Add(esms);
                }
                var responses = await ESmsSendSMS(requestEsms);
                if (responses != null && responses.Any())
                {
                    foreach (var res in responses)
                    {
                        if (res.SmsId.HasValue)
                        {
                            dict.Add(res.SmsId.Value, res.Message);
                        }
                    }
                }
            }
            else if (account.Provider == "vietguys")
            {
                var requestVietguys = new List<VietguysSendMessageRequest>();
                foreach (var line in lines)
                {
                    var vietguys = new VietguysSendMessageRequest();
                    vietguys.phone = line.Partner.Phone;
                    vietguys.LineId = line.Id;
                    vietguys.u = account.UserName;
                    vietguys.pwd = account.Password;
                    vietguys.from = account.BrandName;
                    vietguys.sms = line.Body;
                    vietguys.type = 8;
                    vietguys.json = "1";
                    requestVietguys.Add(vietguys);
                }
                var responses = await VietguysSendSMS(requestVietguys);
                if (responses != null && responses.Any())
                {
                    foreach (var res in responses)
                    {
                        if (res.SmsId.HasValue)
                        {
                            dict.Add(res.SmsId.Value, res.Message);
                        }
                    }
                }
            }
            return dict;
        }

        public async Task<IEnumerable<ESMSSendMessageResponseModel>> ESmsSendSMS(IEnumerable<ESMSSendMessageRequestModel> vals)
        {
            var listReponse = new List<ESMSSendMessageResponseModel>();
            foreach (var val in vals.ToList())
            {
                var model = new ESMSSendMessageResponseModel();
                var url = "http://rest.esms.vn/MainService.svc/json/SendMultipleMessage_V4_post_json/";
                var restClient = new RestClient(url);
                var resRequest = new RestRequest().AddJsonBody(val);
                var response = await restClient.ExecutePostAsync<ESMSSendMessageResponseModel>(resRequest);
                if (response.IsSuccessful)
                {
                    model = response.Data;
                    model.SmsId = val.SmsId;
                    listReponse.Add(model);
                    if (!model.CodeResult.Equals("100"))
                    {
                        model.Message = "fails";
                    }
                    else
                    {
                        model.Message = "success";
                    }
                }
                else
                {
                    model.Message = "fails";
                    model.SmsId = val.SmsId;
                }

            }
            return listReponse;
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

        public async Task<string> ChangeBody(CatalogDbContext context, SmsTemplate template, IEnumerable<Guid> partnerIds)
        {
            var partnerDictTitle = await context.Partners.Where(x => partnerIds.Contains(x.Id)).Include(x => x.Title).ToDictionaryAsync(x => x.Id, x => x.Title.Name);

            var body = JsonConvert.DeserializeObject<Body>(template.Body);

            return "";
        }

        public class Body
        {
            public string TemplateType { get; set; }
            public string Text { get; set; }
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
            public Guid? SmsId { get; set; }
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
    }
}
