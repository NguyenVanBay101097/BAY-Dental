using ApplicationCore.Entities;
using Hangfire;
using Infrastructure.Data;
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
            var section = _configuration.GetSection("ConnectionStrings");
            var catalogConnection = section["CatalogConnection"];
            if (db != "localhost")
                catalogConnection = catalogConnection.Substring(0, catalogConnection.LastIndexOf('_')) + db;

            DbContextOptionsBuilder<CatalogDbContext> builder = new DbContextOptionsBuilder<CatalogDbContext>();
            builder.UseSqlServer(catalogConnection);

            await using var context = new CatalogDbContext(builder.Options, null, null);

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
            var section = _configuration.GetSection("ConnectionStrings");
            var catalogConnection = section["CatalogConnection"];
            if (db != "localhost")
                catalogConnection = catalogConnection.Substring(0, catalogConnection.LastIndexOf('_')) + db;

            DbContextOptionsBuilder<CatalogDbContext> builder = new DbContextOptionsBuilder<CatalogDbContext>();
            builder.UseSqlServer(catalogConnection);

            await using var context = new CatalogDbContext(builder.Options, null, null);
            await using var transaction = await context.Database.BeginTransactionAsync();

            try
            {
                var message = await context.TCareMessages.Where(x => x.Id == id)
                    .Include(x => x.ChannelSocical).Include(x => x.ProfilePartner).FirstOrDefaultAsync();
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
                    }
                }

                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                // TODO: Handle failure
                await transaction.RollbackAsync();
            }
        }
    }
}
