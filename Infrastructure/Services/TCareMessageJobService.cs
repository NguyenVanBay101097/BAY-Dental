using ApplicationCore.Entities;
using Hangfire;
using Infrastructure.Data;
using Infrastructure.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Web.Models.ContentEditing;

namespace Infrastructure.Services
{
    public class TCareMessageJobService
    {
        private readonly IConfiguration _configuration;
        private readonly IFacebookMessageSender _fbMessageSender;
        private readonly ZaloMessageSender _zaloMessageSender;

        public TCareMessageJobService(IConfiguration configuration)
        {
            _configuration = configuration;
            _fbMessageSender = new FacebookMessageSender();
            _zaloMessageSender = new ZaloMessageSender();
        }

        public async Task Run(string db)
        {
            await using var context = DbContextHelper.GetCatalogDbContext(db, _configuration);

            //giới hạn 10000 tin nhắn
            var now = DateTime.Now;
            var messageIds = await context.TCareMessages.Where(x => x.State == "waiting" && (x.ScheduledDate.HasValue || x.ScheduledDate.Value < now))
                .OrderBy(x => x.DateCreated).Take(10000).Select(x => x.Id).ToListAsync();

            foreach (var messageId in messageIds)
            {
                BackgroundJob.Enqueue(() => Send(messageId, db));
            }
        }

        public async Task Send(Guid id, string db)
        {
            await using var context = DbContextHelper.GetCatalogDbContext(db, _configuration);
            await using var transaction = await context.Database.BeginTransactionAsync();

            try
            {
                var message = await context.TCareMessages.Where(x => x.Id == id)
                    .Include(x => x.ChannelSocical).Include(x => x.ProfilePartner)
                    .Include(x => x.Campaign).FirstOrDefaultAsync();
                if (message == null)
                    return;

                var channel = message.ChannelSocical;
                var profile = message.ProfilePartner;

                var now = DateTime.Now;
                var now2 = new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, now.Second);
                if (channel.Type == "facebook")
                {
                    //có cách nào lấy đúng Sent???
                    message.Sent = now2.AddSeconds(-2);
                    var sendResult = await _fbMessageSender.SendMessageTCareTextAsync(message.MessageContent, profile.PSID, channel.PageAccesstoken);
                    if (!string.IsNullOrEmpty(sendResult.error))
                    {
                        message.State = "exception";
                        message.Sent = null;
                        message.FailureReason = sendResult.error;                      
                    }
                    else
                    {
                        message.State = "sent";
                        message.MessageId = sendResult.message_id;

                        await ProcessAfterSent(message, context);
                    }

                    await context.SaveChangesAsync();
                }
                else if (channel.Type == "zalo")
                {
                    message.Sent = now2.AddSeconds(-2);
                    var sendResult = _zaloMessageSender.SendText(message.MessageContent, channel.PageAccesstoken, profile.PSID);
                    if (sendResult.error != 0)
                    {
                        message.State = "exception";
                        message.Sent = null;
                        message.FailureReason = sendResult.message;
                    }
                    else
                    {
                        message.State = "sent";
                        message.MessageId = sendResult.data.message_id;

                        await ProcessAfterSent(message, context);
                    }
                }

                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                // TODO: Handle failure
                await transaction.RollbackAsync();
                throw ex;
            }
        }

        public async Task ProcessAfterSent(TCareMessage message, CatalogDbContext context)
        {
            //xử lý gán nhãn cho những người sẽ gửi
            if (message.Campaign != null && message.Campaign.TagId.HasValue)
            {
                if (message.PartnerId.HasValue)
                {
                    var partnerId = message.PartnerId.Value;
                    var partner = await context.Partners.Where(x => x.Id == partnerId).Include(x => x.PartnerPartnerCategoryRels).FirstOrDefaultAsync();
                    if (partner != null)
                    {
                        var tagId = message.Campaign.TagId.Value;
                        var tag = await context.PartnerCategories.Where(x => x.Id == tagId).FirstOrDefaultAsync();
                        if (tag != null)
                        {
                            if (!partner.PartnerPartnerCategoryRels.Any(x => x.CategoryId == tagId))
                            {
                                partner.PartnerPartnerCategoryRels.Add(new PartnerPartnerCategoryRel
                                {
                                    CategoryId = tagId,
                                });
                            }

                            await context.SaveChangesAsync();
                        }
                    }
                }
            }

        }
    }
}
