
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

namespace Infrastructure.Services
{
    public class TCareMessagingJobService
    {
        private readonly IConfiguration _configuration;
        private readonly DbContextOptionsBuilder<CatalogDbContext> builder = new DbContextOptionsBuilder<CatalogDbContext>();

        public TCareMessagingJobService(IConfiguration configuration)
        {
            _configuration = configuration;

        }
        public async Task RunJobMessagings(string db)
        {          
            var section = _configuration.GetSection("ConnectionStrings");
            var catalogConnection = section["CatalogConnection"];
            builder.UseSqlServer(catalogConnection);
            await using var context = new CatalogDbContext(builder.Options, null, null);
            await using var transaction = await context.Database.BeginTransactionAsync();

            try
            {
                DateTime now = DateTime.Now;
                //tìm danh sách các chiến dịch đang active
                var messagings = await context.TCareMessagings.Where(x => x.State == "in_queue").ToListAsync();

                foreach (var messaging in messagings)
                {

                    //get all message in messaging check state = 'waiting' send message 
                    var messages = await context.TCareMessages.Where(x => x.TCareMessagingId == messaging.Id && x.State == "waiting").ToListAsync();


                    //Gửi tin nhắn 
                    var tasks = messages.Select(x => SendMessgeToPage(x)).ToList();
                    var limit = 200;
                    var offset = 0;
                    var subTasks = tasks.Skip(offset).Take(limit).ToList();
                    while (subTasks.Any())
                    {
                        await Task.WhenAll(subTasks);
                        offset += limit;
                        subTasks = tasks.Skip(offset).Take(limit).ToList();
                    }

                    await UpdateStateMessagingAsync(messaging.Id);
                }

                await context.SaveChangesAsync();
                // Commit transaction if all commands succeed, transaction will auto-rollback
                // when disposed if either commands fails
                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {

                // TODO: Handle failure
                await transaction.RollbackAsync();
            }
        }

        public async Task SendMessgeToPage(TCareMessage mess)
        {

            try
            {
                var partner = await GetPartner(mess.PartnerId.Value);
                var partnerProfile = await GetPartnerProfile(mess.ProfilePartnerId.Value);
                var campaign = await GetCampaign(mess.CampaignId.Value);
                var channelSocial = await GetChannel(mess.ChannelSocicalId.Value);
                await SendMessagePage(campaign, mess, partnerProfile, channelSocial);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }

        public async Task<Partner> GetPartner(Guid id)
        {
            var section = _configuration.GetSection("ConnectionStrings");
            var catalogConnection = section["CatalogConnection"];
            builder.UseSqlServer(catalogConnection);
            await using var context = new CatalogDbContext(builder.Options, null, null);
          

            return await context.Partners.Where(x => x.Id == id && x.Customer == true).FirstOrDefaultAsync();
        }

        public async Task<FacebookUserProfile> GetPartnerProfile(Guid id)
        {
            var section = _configuration.GetSection("ConnectionStrings");
            var catalogConnection = section["CatalogConnection"];
            builder.UseSqlServer(catalogConnection);
            await using var context = new CatalogDbContext(builder.Options, null, null);
           
            var partnerProfile = await context.FacebookUserProfiles.Where(x => x.Id == id).FirstOrDefaultAsync();
            return partnerProfile;
        }

        public async Task<TCareCampaign> GetCampaign(Guid campId)
        {
            var section = _configuration.GetSection("ConnectionStrings");
            var catalogConnection = section["CatalogConnection"];
            builder.UseSqlServer(catalogConnection);
            await using var context = new CatalogDbContext(builder.Options, null, null);
          
            var campaign = await context.TCareCampaigns.Where(x => x.Id == campId).FirstOrDefaultAsync();
            return campaign;
        }

        public async Task<FacebookPage> GetChannel(Guid id)
        {
            var section = _configuration.GetSection("ConnectionStrings");
            var catalogConnection = section["CatalogConnection"];
            builder.UseSqlServer(catalogConnection);
            await using var context = new CatalogDbContext(builder.Options, null, null);
          
            return await context.FacebookPages.Where(x => x.Id == id).FirstOrDefaultAsync();
        }

        private async Task SendMessagePage(TCareCampaign campaign, TCareMessage mess, FacebookUserProfile profile, FacebookPage channelSocial)
        {
            var section = _configuration.GetSection("ConnectionStrings");
            var catalogConnection = section["CatalogConnection"];
            builder.UseSqlServer(catalogConnection);
            await using var context = new CatalogDbContext(builder.Options, null, null);
          
            bool check = false;

            //Create TCareMessageTrace
            var trace = new TCareMessagingTrace()
            {
                TCareCampaignId = campaign.Id,
                PSID = profile.PSID,
                PartnerId = profile.PartnerId,
                Type = channelSocial.Type,
                ChannelSocialId = channelSocial.Id,
                DateCreated = DateTime.Now,
                LastUpdated = DateTime.Now,
                TCareMessagingId = mess.TCareMessagingId
            };

            await context.TCareMessingTraces.AddAsync(trace);
            await context.SaveChangesAsync();

            switch (channelSocial.Type)
            {
                case "facebook":
                    check = await SendMessagePageFacebook(profile, mess.MessageContent, channelSocial, trace.Id, mess.Id);
                    if (check)
                    {
                        if (campaign.TagId.HasValue)
                            await SetTagForPartner(profile.PartnerId.Value, campaign.TagId.Value);

                        await UpdateMessage(mess.Id, trace.Id);
                    }
                    break;
                case "zalo":
                    check = await SendMessagePageZalo(profile, mess.MessageContent, channelSocial, trace.Id, mess.Id);
                    if (check)
                    {
                        if (campaign.TagId.HasValue)
                            await SetTagForPartner(profile.PartnerId.Value, campaign.TagId.Value);

                        await UpdateMessage(mess.Id, trace.Id);
                    }
                    break;
                default:
                    break;
            }
        }

        private async Task<bool> SendMessagePageFacebook(FacebookUserProfile profile, string messageContent, FacebookPage channelSocial, Guid trace_Id, Guid mess_Id)
        {
            var section = _configuration.GetSection("ConnectionStrings");
            var catalogConnection = section["CatalogConnection"];
            builder.UseSqlServer(catalogConnection);
            await using var context = new CatalogDbContext(builder.Options, null, null);
           

            var sendResult = await SendMessageTCareTextAsync(messageContent, profile.PSID, channelSocial.PageAccesstoken);
            var trace = await context.TCareMessingTraces.Where(x => x.Id == trace_Id).FirstOrDefaultAsync();
            if (!string.IsNullOrEmpty(sendResult.error))
            {
                if (trace != null)
                {
                    trace.Exception = DateTime.Now;
                    trace.Error = sendResult.error;

                    await context.SaveChangesAsync();
                }

                await UpdateErrorMessage(mess_Id);
               


                return false;
            }
            else
            {
                if (trace != null)
                {
                    trace.MessageId = sendResult.message_id;
                    await context.SaveChangesAsync();

                }         
                BackgroundJob.Enqueue(() => GetInfoMessageFBApi(trace_Id, channelSocial.PageAccesstoken, sendResult.message_id));
                return true;
            }
        }

        private async Task<bool> SendMessagePageZalo(FacebookUserProfile profile, string messageContent, FacebookPage channelSocial, Guid trace_Id, Guid mess_Id)
        {
            var section = _configuration.GetSection("ConnectionStrings");
            var catalogConnection = section["CatalogConnection"];
            builder.UseSqlServer(catalogConnection);
            await using var context = new CatalogDbContext(builder.Options, null, null);

            var zaloClient = new ZaloClient(channelSocial.PageAccesstoken);
            var sendResult = zaloClient.sendTextMessageToUserId(profile.PSID, messageContent).ToObject<SendMessageZaloResponse>();
            var trace = await context.TCareMessingTraces.Where(x => x.Id == trace_Id).FirstOrDefaultAsync();
            if (sendResult.error != 0)
            {
                if (trace != null)
                {
                    trace.Exception = DateTime.Now;
                    trace.Error = sendResult.error.ToString();
                    await context.SaveChangesAsync();
                }

                await UpdateErrorMessage(mess_Id);
                return false;
            }
            else
            {
                if (trace != null)
                {
                    trace.Sent = DateTime.Now;
                    trace.MessageId = sendResult.data.message_id;
                    await context.SaveChangesAsync();

                }              
                return true;
            }
        }

        [AutomaticRetry]
        public async Task GetInfoMessageFBApi(Guid trace_id, string access_token, string message_id)
        {
            var section = _configuration.GetSection("ConnectionStrings");
            var catalogConnection = section["CatalogConnection"];
            builder.UseSqlServer(catalogConnection);
            await using var context = new CatalogDbContext(builder.Options, null, null);
           
            //lấy thông tin message từ facebook api rồi cập nhật cho message log
            var apiClient = new ApiClient(access_token, FacebookApiVersions.V6_0);
            var url = $"/{message_id}";

            var request = (IGetRequest)ApiRequest.Create(ApiRequest.RequestType.Get, url, apiClient);
            request.AddQueryParameter("fields", "id,message,created_time");
            var response = await request.ExecuteAsync<FacebookMessageApiInfoReponse>();
            if (!response.GetExceptions().Any())
            {
                var result = response.GetResult();

                var trace = await context.TCareMessingTraces.Where(x => x.Id == trace_id).FirstOrDefaultAsync();
                if (trace != null)
                {
                    trace.Sent = result.created_time.ToLocalTime();
                    await context.SaveChangesAsync();
                }

                

            }
        }

        public async Task UpdateStateMessagingAsync(Guid id)
        {
            var section = _configuration.GetSection("ConnectionStrings");
            var catalogConnection = section["CatalogConnection"];
            builder.UseSqlServer(catalogConnection);
            await using var context = new CatalogDbContext(builder.Options, null, null);

            var messaging = await context.TCareMessagings.Where(x => x.Id == id).FirstOrDefaultAsync();
            if (messaging != null)
            {
                messaging.State = "done";

                await context.SaveChangesAsync();
            }
           
        }

        public async Task SetTagForPartner(Guid partnerId, Guid TagId)
        {
            var section = _configuration.GetSection("ConnectionStrings");
            var catalogConnection = section["CatalogConnection"];
            builder.UseSqlServer(catalogConnection);
            await using var context = new CatalogDbContext(builder.Options, null, null);

            var rel = await context.PartnerPartnerCategoryRels.Where(x => x.PartnerId == partnerId && x.CategoryId == TagId).FirstOrDefaultAsync();
            if (rel == null)
            {
                rel = new PartnerPartnerCategoryRel()
                {
                    CategoryId = TagId,
                    PartnerId = partnerId
                };

                await context.PartnerPartnerCategoryRels.AddAsync(rel);
                await context.SaveChangesAsync();
            }


        }

        public async Task UpdateMessage(Guid id, Guid? messagingTraceId)
        {
            var section = _configuration.GetSection("ConnectionStrings");
            var catalogConnection = section["CatalogConnection"];
            builder.UseSqlServer(catalogConnection);
            await using var context = new CatalogDbContext(builder.Options, null, null);

            var trace = await context.TCareMessages.Where(x => x.Id == id).FirstOrDefaultAsync();
            if (trace != null)
            {
                trace.State = "done";
                trace.TCareMessagingTraceId = messagingTraceId.Value;
                await context.SaveChangesAsync();
            }

            
        }

        public async Task UpdateErrorMessage(Guid id)
        {
            var section = _configuration.GetSection("ConnectionStrings");
            var catalogConnection = section["CatalogConnection"];
            builder.UseSqlServer(catalogConnection);
            await using var context = new CatalogDbContext(builder.Options, null, null);

            var trace = await context.TCareMessages.Where(x => x.Id == id).FirstOrDefaultAsync();
            if (trace != null)
            {
                trace.State = "exception";
                await context.SaveChangesAsync();
            }

           
        }

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
}
