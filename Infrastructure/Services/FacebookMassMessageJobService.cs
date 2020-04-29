using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using ApplicationCore.Models;
using ApplicationCore.Specifications;
using ApplicationCore.Utilities;
using Dapper;
using Infrastructure.Data;
using Microsoft.Extensions.Options;
using MyERP.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class FacebookMassMessageJobService : IFacebookMassMessageJobService
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

                    var profiles = GetProfilesSendMessage(messaging, page, conn);
                    if (profiles == null)
                        return;

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
        private IEnumerable<FacebookUserProfile> GetProfilesSendMessage(FacebookMassMessaging self, FacebookPage page, SqlConnection conn = null)
        {

            //Lấy ra những profiles sẽ gửi message
            var builder = new SqlBuilder();
            var sqltemplate = builder.AddTemplate("SELECT /**select**/ FROM FacebookUserProfiles us /**leftjoin**/ /**where**/ /**orderby**/ ");
            builder.Select("us.* ");
            builder.Where("us.FbPageId = @pageId ", new { pageId = page.Id });

            if (!string.IsNullOrEmpty(self.AudienceFilter))
            {
                var filter = JsonConvert.DeserializeObject<SimpleFilter>(self.AudienceFilter);
                if (filter.items.Any())
                {

                    var lst = filter.items.ToDictionary(x => x.type,x=>x);                 
                        foreach (var kvp in lst)
                        {
                            DynamicParameters parameters = new DynamicParameters();
                            if (kvp.Key == "Name" || kvp.Key == "FirstName" || kvp.Key == "LastName")
                            {

                                switch (kvp.Value.formula_type)
                                {
                                    case "contains":
                                    case "eq":
                                        parameters.Add($"@{kvp.Key}", "%" + kvp.Value.formula_value + "%");
                                        builder.Where($"us.{kvp.Key} Like @{kvp.Key} ", parameters);
                                        break;
                                    case "doesnotcontain":
                                    case "neq":
                                        parameters.Add($"@{kvp.Key}", "%" + kvp.Value.formula_value + "%");
                                        builder.Where($"us.{kvp.Key} Not Like @{kvp.Key} ", parameters);
                                        break;
                                    case "startswith":
                                        parameters.Add($"@{kvp.Key}", kvp.Value.formula_value + "%");
                                        builder.Where($"us.{kvp.Key} Like @{kvp.Key} ", parameters);
                                        break;


                                    default:
                                        throw new NotSupportedException(string.Format("Not support Operator {0}!", kvp.Value.formula_type));
                                }
                                //}

                            }
                            else if (kvp.Key == "Gender")
                            {
                                switch (kvp.Value.formula_type)
                                {

                                    case "eq":
                                        builder.Where($"us.{kvp.Key} = @{kvp.Key} ", new { kvp.Value.formula_value });
                                        break;
                                    case "neq":
                                        builder.Where($"us.{kvp.Key} != @{kvp.Key} ", new { kvp.Value.formula_value });
                                        break;
                                    default:
                                        throw new NotSupportedException(string.Format("Not support Operator {0}!", kvp.Value.formula_type));
                                }
                            }
                            else if (kvp.Key == "Tag")
                            {
                                switch (kvp.Value.formula_type)
                                {

                                    case "eq":
                                        builder.LeftJoin("FacebookUserProfileTagRels as rel  On rel.UserProfileId = us.Id ");
                                        builder.LeftJoin("FacebookTags tag ON tag.Id = rel.TagId ");
                                        builder.Where("tag.Name = @TagName ", new { TagName = kvp.Value.formula_value });
                                        break;
                                    case "neq":
                                        builder.LeftJoin("FacebookUserProfileTagRels as rel  On rel.UserProfileId = us.Id ");
                                        builder.LeftJoin("FacebookTags tag ON tag.Id = rel.TagId ");
                                        builder.Where("tag.Name != @TagName ", new { TagName = kvp.Value.formula_value });
                                        break;
                                    default:
                                        throw new NotSupportedException(string.Format("Not support Operator {0}!", kvp.Value.formula_type));
                                }
                            }

                        }
                    


                }
            }

            var iUserprofiles = conn.Query<FacebookUserProfile>(sqltemplate.RawSql, sqltemplate.Parameters).ToList();

            return iUserprofiles;
        }

        public async Task SendMessageAndTrace(SqlConnection conn, Guid mass_messaging_id, string text, FacebookUserProfile profile, string access_token)
        {
            var now = DateTime.Now;
            var date = new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, now.Second);
            date = date.AddSeconds(-1);

            var sendResult = await _fbMessageSender.SendMessageTagTextAsync(text, profile.PSID, access_token);
            if (sendResult == null)
                await conn.ExecuteAsync("insert into FacebookMessagingTraces(Id,MassMessagingId,Exception,UserProfileId) values (@Id,@MassMessagingId,@Exception,@UserProfileId)", new { Id = GuidComb.GenerateComb(), MassMessagingId = mass_messaging_id, Exception = date, UserProfileId = profile.Id });
            else
                await conn.ExecuteAsync("insert into FacebookMessagingTraces(Id,MassMessagingId,Sent,MessageId,UserProfileId) values (@Id,@MassMessagingId,@Sent,@MessageId,@UserProfileId)", new { Id = GuidComb.GenerateComb(), MassMessagingId = mass_messaging_id, Sent = date, MessageId = sendResult.message_id, UserProfileId = profile.Id });
        }
    }
}
