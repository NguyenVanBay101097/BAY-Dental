using ApplicationCore.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using MyERP.Utilities;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class SmsSendMessageService : ISmsSendMessageService
    {
        public async Task CreateSmsMessageDetail(CatalogDbContext context, SmsMessage smsMessage, IEnumerable<Guid> ids, Guid companyId)
        {
            var listSms = new List<SmsMessageDetail>();
            var dictApp = new Dictionary<Guid, Appointment>();
            var partners = await context.Partners.Where(x => ids.Contains(x.Id)).Include(x => x.Title).ToListAsync();
            if (smsMessage.ResModel == "appointment")
            {
                var appIds = smsMessage.SmsMessageAppointmentRels.Select(x => x.AppointmentId).ToList();
                dictApp = await context.Appointments.Where(x => appIds.Contains(x.Id)).Include(x => x.Doctor).ToDictionaryAsync(x => x.PartnerId, x => x);
            }
            var company = await context.Companies.Where(x => x.Id == companyId).FirstOrDefaultAsync();
            foreach (var partner in partners)
            {
                var content = await PersonalizedContent(context, smsMessage.Body, partner, company, dictApp);
                var sms = new SmsMessageDetail();
                sms.Id = GuidComb.GenerateComb();
                sms.Body = content;
                sms.Number = !string.IsNullOrEmpty(partner.Phone) ? partner.Phone : "";
                sms.PartnerId = partner.Id;
                sms.State = "sending";
                sms.SmsAccountId = smsMessage.SmsAccountId.Value;
                sms.SmsMessageId = smsMessage.Id;
                sms.SmsCampaignId = smsMessage.SmsCampaignId;
                listSms.Add(sms);
            }

            await context.SmsMessageDetails.AddRangeAsync(listSms);
            await context.SaveChangesAsync();

            var dict = await SendSMS(listSms, smsMessage.SmsAccount, context);
            foreach (var item in listSms)
            {
                if (dict.ContainsKey(item.Id))
                {
                    item.State = dict[item.Id].Message;
                    item.ErrorCode = dict[item.Id].CodeResult;
                    context.Entry(item).State = EntityState.Modified;
                }
            }
            var messError = dict.Values.Where(x => x.CodeResult != "100");
            if (messError == null || !messError.Any())
            {
                smsMessage.State = "success";
                context.Entry(smsMessage).State = EntityState.Modified;
            }
            await context.SaveChangesAsync();
        }


        private async Task<string> PersonalizedContent(
            CatalogDbContext context, string body, Partner partner, Company company, Dictionary<Guid, Appointment> dictApp)
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

        public async Task<Dictionary<Guid, ESMSSendMessageResponseModel>> SendSMS(IEnumerable<SmsMessageDetail> lines, SmsAccount account, CatalogDbContext context)
        {
            var dict = new Dictionary<Guid, ESMSSendMessageResponseModel>();
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
                            //dict.Add(res.SmsId.Value, res);
                        }
                    }
                }
            }
            else if (account.Provider == "esms")
            {
                var requestEsms = new List<ESMSSendMessageRequestModel>();
                foreach (var line in lines)
                {
                    var esms = new ESMSSendMessageRequestModel();
                    esms.Phone = line.Number;
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
                        if (res.SmsSmsId.HasValue)
                        {
                            dict.Add(res.SmsSmsId.Value, res);
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
                model.SmsSmsId = val.SmsId;
                listReponse.Add(model);
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
            public Guid? SmsSmsId { get; set; }
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
