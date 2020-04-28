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
using System.Linq;
using System.Linq.Expressions;
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

                    var profiles = GetProfilesSendMessage(messaging,page,conn);
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
            ISpecification<FacebookUserProfile> profileSpec = new InitialSpecification<FacebookUserProfile>(x => x.FbPageId == page.Id);
            if (!string.IsNullOrEmpty(self.AudienceFilter))
            {
                var filter = JsonConvert.DeserializeObject<SimpleFilter>(self.AudienceFilter);
                if (filter.items.Any())
                {
                    var itemSpecs = new List<ISpecification<FacebookUserProfile>>();
                    foreach (var item in filter.items)
                    {
                        var parameter = Expression.Parameter(typeof(FacebookUserProfile), "x");
                        var expression = ToLamdaExpression(item, parameter);

                        var predicateExpression = Expression.Lambda<Func<FacebookUserProfile, bool>>(expression, parameter);
                        itemSpecs.Add(new InitialSpecification<FacebookUserProfile>(predicateExpression));
                    }

                    if (filter.type == "and")
                    {
                        ISpecification<FacebookUserProfile> tmp = new InitialSpecification<FacebookUserProfile>(x => true);
                        foreach (var spec in itemSpecs)
                            tmp = tmp.And(spec);
                        profileSpec = profileSpec.And(tmp);
                    }
                    else
                    {
                        ISpecification<FacebookUserProfile> tmp = new InitialSpecification<FacebookUserProfile>(x => false);
                        foreach (var spec in itemSpecs)
                            tmp = tmp.Or(spec);
                        profileSpec = profileSpec.And(tmp);
                    }
                }
            }
            var userProfiles = conn.Query<FacebookUserProfile>("" +
                             "SELECT * " +
                             "FROM FacebookUserProfiles us " +
                             "where us.FbPageId = @pageId  " +
                             "", new { pageId = page.Id }).ToList();
            var query = userProfiles.AsQueryable();
            var result = query.Where(profileSpec.AsExpression()).ToList();
            return result;
        }

        private Expression ToLamdaExpression(SimpleFilterItem item, ParameterExpression parameter)
        {
            Expression resultExpression = null;
            if (item.type == "Name" || item.type == "FirstName" || item.type == "LastName" || item.type == "Gender")
            {
                Expression left = Expression.PropertyOrField(parameter, item.type);
                Expression right = Expression.Constant(item.formula_value);
                switch (item.formula_type)
                {
                    case "contains":
                    case "doesnotcontain":
                    case "startswith":
                        var nullCheckExpression = Expression.Equal(left, Expression.Constant(null, typeof(String)));

                        if (item.formula_type == "contains" || item.formula_type == "doesnotcontain")
                        {
                            var containsMethod = typeof(String).GetMethod("Contains", new[] { typeof(String) });
                            var containsExpression = Expression.Call(left, containsMethod, right);
                            if (item.formula_type == "contains")
                                resultExpression = Expression.AndAlso(Expression.Not(nullCheckExpression), containsExpression);
                            else
                                resultExpression = Expression.AndAlso(Expression.Not(nullCheckExpression), Expression.Not(containsExpression));
                        }
                        else if (item.formula_type == "startswith")
                        {
                            var startswithMethod = typeof(String).GetMethod("StartsWith", new[] { typeof(String) });
                            var startswithExpression = Expression.Call(left, startswithMethod, right);
                            resultExpression = Expression.AndAlso(Expression.Not(nullCheckExpression), startswithExpression);
                        }
                        break;
                    case "eq":
                    case "neq":
                        var equalCheckExpression = Expression.Equal(left, right);
                        if (item.formula_type == "eq")
                            resultExpression = equalCheckExpression;
                        else
                            resultExpression = Expression.Not(equalCheckExpression);
                        break;
                    default:
                        throw new NotSupportedException(string.Format("Not support Operator {0}!", item.formula_type));
                }
            }
            else if (item.type == "Tag")
            {

                Expression tagRelsExpression = Expression.PropertyOrField(parameter, "TagRels");
                switch (item.formula_type)
                {
                    case "eq":
                    case "neq":
                        // find Any method                       
                        var generic = typeof(Queryable).GetMethods()
                               .Where(m => m.Name == "Any")
                               .Where(m => m.GetParameters().Length == 2)
                               .Single();
                        var containsMethod = generic.MakeGenericMethod(typeof(FacebookUserProfileTagRel));

                        var tagRelParameter = Expression.Parameter(typeof(FacebookUserProfileTagRel), "s");
                        //var tag = Expression.Parameter(typeof(FacebookTag), "Tag");
                        //var propertyTag = Expression.PropertyOrField(tag, "Name");
                        Expression left = Expression.PropertyOrField(tagRelParameter, "Tag");
                        Expression left2 = Expression.PropertyOrField(left, "Name");
                        Expression right = Expression.Constant(item.formula_value);
                        Expression equalExpression = Expression.Equal(left2, right);

                        var predicate = Expression.Lambda<Func<FacebookUserProfileTagRel, bool>>(equalExpression, tagRelParameter);
                        var containsExpression = ExpressionUtils.CallAny(tagRelsExpression, predicate, "Any");
                        if (item.formula_type == "eq")
                            resultExpression = containsExpression;
                        else
                            resultExpression = Expression.Not(containsExpression);
                        break;
                    default:
                        throw new NotSupportedException(string.Format("Not support Operator {0}!", item.formula_type));
                }
            }
            else
                throw new NotSupportedException(string.Format("Not support type {0}!", item.type));

            return resultExpression;
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
