using ApplicationCore.Entities;
using Dapper;
using Infrastructure.Data;
using Microsoft.Extensions.Options;
using MyERP.Utilities;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class FacebookMassMessageJobService: IFacebookMassMessageJobService
    {
        private readonly ConnectionStrings _connectionStrings;
        private readonly IFacebookMessageSender _fbMessageSender;

        public FacebookMassMessageJobService(IOptions<ConnectionStrings> connectionStrings,
            IFacebookMessageSender fbMessageSender)
        {
            _connectionStrings = connectionStrings?.Value;
            _fbMessageSender = fbMessageSender;
        }

        public async Task SendMessage(string db, Guid massMessagingId)
        {
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(_connectionStrings.CatalogConnection);
            builder["Database"] = $"TMTDentalCatalogDb__{db}";
            if (db == "localhost")
                builder["Database"] = "TMTDentalCatalogDb";

            using (var conn = new SqlConnection(builder.ConnectionString))
            {
                try
                {
                    conn.Open();

                    var messaging = conn.Query<FacebookMassMessaging>("" +
                        "SELECT * " +
                        "FROM FacebookMassMessagings " +
                        "where Id = @id" +
                        "", new { id = massMessagingId }).FirstOrDefault();

                    if (messaging == null || string.IsNullOrEmpty(messaging.Content))
                        return;

                    var page = conn.Query<FacebookPage>("" +
                        "SELECT * " +
                        "FROM FacebookPages " +
                        "where Id = @id" +
                        "", new { id = messaging.FacebookPageId }).FirstOrDefault();

                    if (page == null)
                        return;

                    var profiles = conn.Query<FacebookUserProfile>("" +
                          "SELECT * " +
                              "FROM FacebookUserProfiles m " +
                              "where m.FbPageId = @pageId " +
                              "", new { pageId = page.Id }).ToList();

                    var tasks = profiles.Select(x => SendMessageAndTrace(conn, messaging.Id, messaging.Content, x, page.PageAccesstoken)).ToList();
                    await Task.WhenAll(tasks);

                    await conn.ExecuteAsync("update FacebookMassMessagings set State=@state,SentDate=@sentDate where Id=@id", new { state = "done", sentDate = DateTime.Now, id = messaging.Id });
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
        }

        public async Task SendMessageAndTrace(SqlConnection conn, Guid mass_messaging_id, string text, FacebookUserProfile profile, string access_token)
        {
            var sendResult = await _fbMessageSender.SendMessageTagTextAsync(text, profile.PSID, access_token);
            if (sendResult == null)
                await conn.ExecuteAsync("insert into FacebookMessagingTraces(Id,MassMessagingId,Exception,UserProfileId) values (@Id,@MassMessagingId,@Exception,@UserProfileId)", new { Id = GuidComb.GenerateComb(), MassMessagingId = mass_messaging_id, Exception = DateTime.Now, UserProfileId = profile.Id });
            else
                await conn.ExecuteAsync("insert into FacebookMessagingTraces(Id,MassMessagingId,Sent,MessageId,UserProfileId) values (@Id,@MassMessagingId,@Sent,@MessageId,@UserProfileId)", new { Id = GuidComb.GenerateComb(), MassMessagingId = mass_messaging_id, Sent = DateTime.Now, MessageId = sendResult.message_id, UserProfileId = profile.Id });
        }
    }
}
