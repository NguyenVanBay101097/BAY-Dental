
using ApplicationCore.Entities;
using Facebook.ApiClient.ApiEngine;
using Facebook.ApiClient.Constants;
using Facebook.ApiClient.Interfaces;
using Hangfire;
using Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZaloDotNetSDK;
using Antlr4.StringTemplate;
using Antlr4;
using ApplicationCore.Utilities;
using Infrastructure.Helpers;
using System.Xml.Serialization;
using System.IO;
using Umbraco.Web.Models.ContentEditing;

namespace Infrastructure.Services
{
    public class TCareMessagingJobService
    {
        private readonly IConfiguration _configuration;

        public TCareMessagingJobService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task ProcessQueue(string db)
        {
            await using var context = DbContextHelper.GetCatalogDbContext(db, _configuration);

            try
            {
                DateTime now = DateTime.Now;
                //tìm danh sách các chiến dịch đang active
                var states = new string[] { "in_queue", "sending" };
                var messagings = await context.TCareMessagings.Where(x => states.Contains(x.State) && (x.ScheduleDate.HasValue || x.ScheduleDate.Value < now)).ToListAsync();

                foreach (var messaging in messagings)
                {
                    BackgroundJob.Enqueue<TCareMessagingJobService>(x => x.ActionSendMail(db, messaging.Id));
                    ////get all message in messaging check state = 'waiting' send message 
                    //var messages = await context.TCareMessages.Where(x => x.TCareMessagingId == messaging.Id && x.State == "waiting").ToListAsync();


                    ////Gửi tin nhắn 
                    //var tasks = messages.Select(x => SendMessgeToPage(x)).ToList();
                    //var limit = 200;
                    //var offset = 0;
                    //var subTasks = tasks.Skip(offset).Take(limit).ToList();
                    //while (subTasks.Any())
                    //{
                    //    await Task.WhenAll(subTasks);
                    //    offset += limit;
                    //    subTasks = tasks.Skip(offset).Take(limit).ToList();
                    //}

                    //await UpdateStateMessagingAsync(messaging.Id);
                }

                //await context.SaveChangesAsync();
                //// Commit transaction if all commands succeed, transaction will auto-rollback
                //// when disposed if either commands fails
                //await transaction.CommitAsync();
            }
            catch (Exception)
            {
                // TODO: Handle failure
                //await transaction.RollbackAsync();
            }
        }

        public async Task ActionSendMail(string db, Guid messagingId)
        {
            await using var context = DbContextHelper.GetCatalogDbContext(db, _configuration);
            await using var transaction = await context.Database.BeginTransactionAsync();

            try
            {
                //làm sao để gửi tin?
                //điều kiện cần: kênh gửi, danh sách người nhận, nội dung
                var messaging = await context.TCareMessagings.Where(x => x.Id == messagingId).Include(x => x.FacebookPage)
                    .Include(x => x.PartnerRecipients).Include(x => x.TCareCampaign).FirstOrDefaultAsync();
                if (messaging == null)
                    return;

                var channel = messaging.FacebookPage;
                if (channel.Type == "facebook" || channel.Type == "zalo")
                {
                    var partnerIds = messaging.PartnerRecipients.Select(x => x.PartnerId).ToList();
                    var profileDict = await GetProfileDict(partnerIds, channel.Id, context);

                    //lấy data để cá nhân hóa nội dung tin nhắn
                    var send_partner_ids = profileDict.Select(x => x.Key).ToList();
                    var dataPersonalized = await GetDataPersonalized(send_partner_ids, context);
                    var dataPersonalizedDict = dataPersonalized.ToDictionary(x => x.Id, x => x);

                    var messages = new List<TCareMessage>();
                    foreach (var item in profileDict)
                    {
                        var personalized = dataPersonalizedDict[item.Key];

                        //render nội dung tin nhắn
                        var template = new Template(messaging.Content, '{', '}');
                        template.Add("ten_khach_hang", personalized.Name.Split(' ').Last());
                        template.Add("ten_page", channel.PageName);
                        template.Add("danh_xung_khach_hang", personalized.Title);

                        if (messaging.Content.Contains("{ma_coupon}") && messaging.CouponProgramId.HasValue)
                        {
                            template.Add("ma_coupon", await CreateNewCoupon(messaging.CouponProgramId.Value, item.Key, context));
                        }

                        var messageContent = template.Render();

                        var message = new TCareMessage()
                        {
                            ProfilePartnerId = item.Value,
                            ChannelSocicalId = messaging.FacebookPageId,
                            CampaignId = messaging.TCareCampaignId,
                            PartnerId = item.Key,
                            MessageContent = messageContent,
                            TCareMessagingId = messaging.Id,
                            State = "waiting",
                            ScheduledDate = messaging.ScheduleDate,
                        };

                        messages.Add(message);
                    }

                    await context.AddRangeAsync(messages);
                    await context.SaveChangesAsync();

                    var batchStr = BatchJob.StartNew(x =>
                    {
                        foreach (var message in messages)
                        {
                            x.Enqueue<TCareMessageJobService>(x => x.Send(message.Id, db));
                        }
                    });
                }

                messaging.State = "done";
                await context.SaveChangesAsync();

                await transaction.CommitAsync();
            }
            catch (Exception e)
            {
                await transaction.RollbackAsync();
            }
        }

        public async Task<string> CreateNewCoupon(Guid programId, Guid partnerId, CatalogDbContext context)
        {
            var program = await context.SaleCouponPrograms.Where(x => x.Id == programId).FirstOrDefaultAsync();
            if (program == null) return "";
            var coupon = new SaleCoupon
            {
                Code = StringUtils.RandomStringDigit(13),
                ProgramId = programId,
                State = "reserved",
                Program = program,
                PartnerId = partnerId
            };
            // check và tính expire date
            var check = await CheckCodeCoupon(coupon, context);
            if (!check) return "";

            ComputeDateExpireCoupon(coupon, context);
            await context.SaleCoupons.AddAsync(coupon);
            await context.SaveChangesAsync();
            return coupon.Code;
        }
        public async Task<bool> CheckCodeCoupon(SaleCoupon self, CatalogDbContext context, int count = 1)
        {
            var exist = await context.SaleCoupons.AnyAsync(x => x.Code == self.Code);
            if (!exist)
                return true;
            if (count > 3)
                return false;
            self.Code = StringUtils.RandomStringDigit(13);
            return await CheckCodeCoupon(self, context, count + 1);
        }

        public void ComputeDateExpireCoupon(SaleCoupon self, CatalogDbContext context)
        {
            var date = (self.DateCreated ?? DateTime.Today).Date;
            var validity_duration = self.Program.ValidityDuration ?? 0;
            if (validity_duration > 0)
                self.DateExpired = date.AddDays(validity_duration);
            else
                self.DateExpired = null;
        }
        private async Task<IEnumerable<TCareMessagingJobPartnerDataPersonalized>> GetDataPersonalized(IEnumerable<Guid> partner_ids, CatalogDbContext context)
        {
            var limit = 1000;
            var offset = 0;
            var result = new List<TCareMessagingJobPartnerDataPersonalized>();
            while (offset < partner_ids.Count())
            {
                var ids = partner_ids.Skip(offset).Take(limit);
                var items = await context.Partners.Where(x => ids.Contains(x.Id))
                .Select(x => new TCareMessagingJobPartnerDataPersonalized
                {
                    Id = x.Id,
                    Name = x.Name,
                    Title = x.Title != null ? x.Title.Name : string.Empty
                }).ToListAsync();
                result.AddRange(items);

                offset += limit;
            }

            return result;
        }

        private async Task<IDictionary<Guid, Guid>> GetProfileDict(IEnumerable<Guid> partnerIds, Guid? fbFageId, CatalogDbContext context)
        {
            var limit = 1000;
            var offset = 0;
            var result = new Dictionary<Guid, Guid>();
            while (offset < partnerIds.Count())
            {
                var ids = partnerIds.Skip(offset).Take(limit);
                var items = await context.FacebookUserProfiles.Where(x => x.PartnerId.HasValue && ids.Contains(x.Partner.Id) && x.FbPageId == fbFageId)
                .Select(x => new
                {
                    Id = x.Id,
                    PartnerId = x.PartnerId.Value
                }).ToListAsync();

                foreach (var item in items)
                {
                    if (!result.ContainsKey(item.PartnerId))
                        result.Add(item.PartnerId, item.Id);
                }

                offset += limit;
            }

            return result;
        }


        //public async Task SendMessgeToPage(TCareMessage mess)
        //{

        //    try
        //    {
        //        var partner = await GetPartner(mess.PartnerId.Value);
        //        var partnerProfile = await GetPartnerProfile(mess.ProfilePartnerId.Value);
        //        var campaign = await GetCampaign(mess.CampaignId.Value);
        //        var channelSocial = await GetChannel(mess.ChannelSocicalId.Value);
        //        await SendMessagePage(campaign, mess, partnerProfile, channelSocial);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception(ex.Message);
        //    }

        //}

        //public async Task<Partner> GetPartner(Guid id)
        //{
        //    var section = _configuration.GetSection("ConnectionStrings");
        //    var catalogConnection = section["CatalogConnection"];
        //    builder.UseSqlServer(catalogConnection);
        //    await using var context = new CatalogDbContext(builder.Options, null, null);


        //    return await context.Partners.Where(x => x.Id == id && x.Customer == true).FirstOrDefaultAsync();
        //}

        //public async Task<FacebookUserProfile> GetPartnerProfile(Guid id)
        //{
        //    var section = _configuration.GetSection("ConnectionStrings");
        //    var catalogConnection = section["CatalogConnection"];
        //    builder.UseSqlServer(catalogConnection);
        //    await using var context = new CatalogDbContext(builder.Options, null, null);

        //    var partnerProfile = await context.FacebookUserProfiles.Where(x => x.Id == id).FirstOrDefaultAsync();
        //    return partnerProfile;
        //}

        //public async Task<TCareCampaign> GetCampaign(Guid campId)
        //{
        //    var section = _configuration.GetSection("ConnectionStrings");
        //    var catalogConnection = section["CatalogConnection"];
        //    builder.UseSqlServer(catalogConnection);
        //    await using var context = new CatalogDbContext(builder.Options, null, null);

        //    var campaign = await context.TCareCampaigns.Where(x => x.Id == campId).FirstOrDefaultAsync();
        //    return campaign;
        //}

        //public async Task<FacebookPage> GetChannel(Guid id)
        //{
        //    var section = _configuration.GetSection("ConnectionStrings");
        //    var catalogConnection = section["CatalogConnection"];
        //    builder.UseSqlServer(catalogConnection);
        //    await using var context = new CatalogDbContext(builder.Options, null, null);

        //    return await context.FacebookPages.Where(x => x.Id == id).FirstOrDefaultAsync();
        //}

        //private async Task SendMessagePage(TCareCampaign campaign, TCareMessage mess, FacebookUserProfile profile, FacebookPage channelSocial)
        //{
        //    var section = _configuration.GetSection("ConnectionStrings");
        //    var catalogConnection = section["CatalogConnection"];
        //    builder.UseSqlServer(catalogConnection);
        //    await using var context = new CatalogDbContext(builder.Options, null, null);

        //    bool check = false;

        //    //Create TCareMessageTrace
        //    //var trace = new TCareMessagingTrace()
        //    //{
        //    //    TCareCampaignId = campaign.Id,
        //    //    PSID = profile.PSID,
        //    //    PartnerId = profile.PartnerId,
        //    //    Type = channelSocial.Type,
        //    //    ChannelSocialId = channelSocial.Id,
        //    //    DateCreated = DateTime.Now,
        //    //    LastUpdated = DateTime.Now,
        //    //    TCareMessagingId = mess.TCareMessagingId
        //    //};

        //    //await context.TCareMessingTraces.AddAsync(trace);
        //    await context.SaveChangesAsync();

        //    //switch (channelSocial.Type)
        //    //{
        //    //    case "facebook":
        //    //        check = await SendMessagePageFacebook(profile, mess.MessageContent, channelSocial, trace.Id, mess.Id);
        //    //        if (check)
        //    //        {
        //    //            if (campaign.TagId.HasValue)
        //    //                await SetTagForPartner(profile.PartnerId.Value, campaign.TagId.Value);

        //    //            await UpdateMessage(mess.Id, trace.Id);
        //    //        }
        //    //        break;
        //    //    case "zalo":
        //    //        check = await SendMessagePageZalo(profile, mess.MessageContent, channelSocial, trace.Id, mess.Id);
        //    //        if (check)
        //    //        {
        //    //            if (campaign.TagId.HasValue)
        //    //                await SetTagForPartner(profile.PartnerId.Value, campaign.TagId.Value);

        //    //            await UpdateMessage(mess.Id, trace.Id);
        //    //        }
        //    //        break;
        //    //    default:
        //    //        break;
        //    //}
        //}

        //private async Task<bool> SendMessagePageFacebook(FacebookUserProfile profile, string messageContent, FacebookPage channelSocial, Guid trace_Id, Guid mess_Id)
        //{
        //    var section = _configuration.GetSection("ConnectionStrings");
        //    var catalogConnection = section["CatalogConnection"];
        //    builder.UseSqlServer(catalogConnection);
        //    await using var context = new CatalogDbContext(builder.Options, null, null);


        //    var sendResult = await SendMessageTCareTextAsync(messageContent, profile.PSID, channelSocial.PageAccesstoken);
        //    //var trace = await context.TCareMessingTraces.Where(x => x.Id == trace_Id).FirstOrDefaultAsync();
        //    //if (!string.IsNullOrEmpty(sendResult.error))
        //    //{
        //    //    if (trace != null)
        //    //    {
        //    //        trace.Exception = DateTime.Now;
        //    //        trace.Error = sendResult.error;

        //    //        await context.SaveChangesAsync();
        //    //    }

        //    //    await UpdateErrorMessage(mess_Id);



        //    //    return false;
        //    //}
        //    //else
        //    //{
        //    //    if (trace != null)
        //    //    {
        //    //        trace.MessageId = sendResult.message_id;
        //    //        await context.SaveChangesAsync();

        //    //    }         
        //    //    BackgroundJob.Enqueue(() => GetInfoMessageFBApi(trace_Id, channelSocial.PageAccesstoken, sendResult.message_id));
        //    //    return true;
        //    //}

        //    return true;
        //}

        //private async Task<bool> SendMessagePageZalo(FacebookUserProfile profile, string messageContent, FacebookPage channelSocial, Guid trace_Id, Guid mess_Id)
        //{
        //    var section = _configuration.GetSection("ConnectionStrings");
        //    var catalogConnection = section["CatalogConnection"];
        //    builder.UseSqlServer(catalogConnection);
        //    await using var context = new CatalogDbContext(builder.Options, null, null);

        //    var zaloClient = new ZaloClient(channelSocial.PageAccesstoken);
        //    var sendResult = zaloClient.sendTextMessageToUserId(profile.PSID, messageContent).ToObject<SendMessageZaloResponse>();
        //    //var trace = await context.TCareMessingTraces.Where(x => x.Id == trace_Id).FirstOrDefaultAsync();
        //    //if (sendResult.error != 0)
        //    //{
        //    //    if (trace != null)
        //    //    {
        //    //        trace.Exception = DateTime.Now;
        //    //        trace.Error = sendResult.error.ToString();
        //    //        await context.SaveChangesAsync();
        //    //    }

        //    //    await UpdateErrorMessage(mess_Id);
        //    //    return false;
        //    //}
        //    //else
        //    //{
        //    //    if (trace != null)
        //    //    {
        //    //        trace.Sent = DateTime.Now;
        //    //        trace.MessageId = sendResult.data.message_id;
        //    //        await context.SaveChangesAsync();

        //    //    }              
        //    //    return true;
        //    //}
        //    return true;
        //}

        //[AutomaticRetry]
        //public async Task GetInfoMessageFBApi(Guid trace_id, string access_token, string message_id)
        //{
        //    var section = _configuration.GetSection("ConnectionStrings");
        //    var catalogConnection = section["CatalogConnection"];
        //    builder.UseSqlServer(catalogConnection);
        //    await using var context = new CatalogDbContext(builder.Options, null, null);

        //    //lấy thông tin message từ facebook api rồi cập nhật cho message log
        //    var apiClient = new ApiClient(access_token, FacebookApiVersions.V6_0);
        //    var url = $"/{message_id}";

        //    var request = (IGetRequest)ApiRequest.Create(ApiRequest.RequestType.Get, url, apiClient);
        //    request.AddQueryParameter("fields", "id,message,created_time");
        //    var response = await request.ExecuteAsync<FacebookMessageApiInfoReponse>();
        //    if (!response.GetExceptions().Any())
        //    {
        //        var result = response.GetResult();

        //        //var trace = await context.TCareMessingTraces.Where(x => x.Id == trace_id).FirstOrDefaultAsync();
        //        //if (trace != null)
        //        //{
        //        //    trace.Sent = result.created_time.ToLocalTime();
        //        //    await context.SaveChangesAsync();
        //        //}



        //    }
        //}

        //public async Task UpdateStateMessagingAsync(Guid id)
        //{
        //    var section = _configuration.GetSection("ConnectionStrings");
        //    var catalogConnection = section["CatalogConnection"];
        //    builder.UseSqlServer(catalogConnection);
        //    await using var context = new CatalogDbContext(builder.Options, null, null);

        //    var messaging = await context.TCareMessagings.Where(x => x.Id == id).FirstOrDefaultAsync();
        //    if (messaging != null)
        //    {
        //        messaging.State = "done";

        //        await context.SaveChangesAsync();
        //    }

        //}

        //public async Task SetTagForPartner(Guid partnerId, Guid TagId)
        //{
        //    var section = _configuration.GetSection("ConnectionStrings");
        //    var catalogConnection = section["CatalogConnection"];
        //    builder.UseSqlServer(catalogConnection);
        //    await using var context = new CatalogDbContext(builder.Options, null, null);

        //    var rel = await context.PartnerPartnerCategoryRels.Where(x => x.PartnerId == partnerId && x.CategoryId == TagId).FirstOrDefaultAsync();
        //    if (rel == null)
        //    {
        //        rel = new PartnerPartnerCategoryRel()
        //        {
        //            CategoryId = TagId,
        //            PartnerId = partnerId
        //        };

        //        await context.PartnerPartnerCategoryRels.AddAsync(rel);
        //        await context.SaveChangesAsync();
        //    }


        //}

        //public async Task UpdateMessage(Guid id, Guid? messagingTraceId)
        //{
        //    var section = _configuration.GetSection("ConnectionStrings");
        //    var catalogConnection = section["CatalogConnection"];
        //    builder.UseSqlServer(catalogConnection);
        //    await using var context = new CatalogDbContext(builder.Options, null, null);

        //    var trace = await context.TCareMessages.Where(x => x.Id == id).FirstOrDefaultAsync();
        //    //if (trace != null)
        //    //{
        //    //    trace.State = "done";
        //    //    trace.TCareMessagingTraceId = messagingTraceId.Value;
        //    //    await context.SaveChangesAsync();
        //    //}


        //}

        //public async Task UpdateErrorMessage(Guid id)
        //{
        //    var section = _configuration.GetSection("ConnectionStrings");
        //    var catalogConnection = section["CatalogConnection"];
        //    builder.UseSqlServer(catalogConnection);
        //    await using var context = new CatalogDbContext(builder.Options, null, null);

        //    var trace = await context.TCareMessages.Where(x => x.Id == id).FirstOrDefaultAsync();
        //    if (trace != null)
        //    {
        //        trace.State = "exception";
        //        await context.SaveChangesAsync();
        //    }


        //}

        public async Task<SendFacebookMessageReponse> SendMessageTCareTextAsync(string message, string psid, string access_token, string tag = "ACCOUNT_UPDATE")
        {
            var apiClient = new ApiClient(access_token, FacebookApiVersions.V6_0);
            var url = $"/me/messages";

            var request = (IPostRequest)ApiRequest.Create(ApiRequest.RequestType.Post, url, apiClient);
            request.AddParameter("message_type", "MESSAGE_TAG");
            request.AddParameter("recipient", JsonConvert.SerializeObject(new { id = psid }));
            request.AddParameter("message", JsonConvert.SerializeObject(new { text = message }));
            request.AddParameter("tag", tag);

            var response = await request.ExecuteAsync<SendFacebookMessageReponse>();
            if (response.GetExceptions().Any())
            {
                return new SendFacebookMessageReponse
                {
                    error = string.Join(";", response.GetExceptions().Select(x => x.Message))
                };
            }
            else
            {
                var result = response.GetResult();
                return result;
            }
        }
    }

    public class TCareMessagingJobPartnerDataPersonalized
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Title { get; set; }
    }
}
