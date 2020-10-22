using ApplicationCore.Entities;
using Infrastructure.Data;
using Infrastructure.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using Umbraco.Web.Models.ContentEditing;
using ZaloCSharpSDK;

namespace Infrastructure.Services
{
    public class TCareCampaignJobService
    {
        private readonly IConfiguration _configuration;
        public TCareCampaignJobService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task Run(string db)
        {
            await using var context = DbContextHelper.GetCatalogDbContext(db, _configuration);
            await using var transaction = await context.Database.BeginTransactionAsync();

            try
            {
                var activeCampaigns = await context.TCareCampaigns.Where(x => x.Active).ToListAsync();
                foreach (var campaign in activeCampaigns)
                {
                    if (!campaign.FacebookPageId.HasValue)
                        continue;

                    //với mỗi chiến dịch lọc danh sách khách hàng
                    var partner_ids = await SearchPartnerIds(campaign.GraphXml, context);
                    if (partner_ids == null || !partner_ids.Any())
                        continue;

                    var profiles = await GetProfiles(partner_ids, campaign.FacebookPageId.Value, context);

                    var content = GetCampaignContent(campaign.GraphXml);
                    if (string.IsNullOrEmpty(content))
                        continue;

                    //tạo tin nhắn gửi hàng loạt đưa vào hàng đợi
                    var scheduleDate = GetScheduleDateCampaign(campaign);
                    var messaging = new TCareMessaging()
                    {
                        ScheduleDate = scheduleDate,
                        State = "in_queue",
                        Content = content,
                        TCareCampaignId = campaign.Id,
                        FacebookPageId = campaign.FacebookPageId,
                    };

                    var outCouponId = Guid.NewGuid();
                    if (Guid.TryParse(GetCampaignCouponId(campaign.GraphXml), out outCouponId)) messaging.CouponProgramId = outCouponId;

                    foreach (var profile in profiles)
                    {
                        messaging.PartnerRecipients.Add(new TCareMessagingPartnerRel
                        {
                            PartnerId = profile.PartnerId.Value
                        });
                    }

                    await context.TCareMessagings.AddAsync(messaging);
                    await context.SaveChangesAsync();

                    //Gửi tin nhắn hàng loạt
                    //await CreateMessages(context, messaging, partner_ids);
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

        private async Task<IEnumerable<FacebookUserProfile>> GetProfiles(IEnumerable<Guid> partnerIds, Guid? fbFageId, CatalogDbContext context)
        {
            var limit = 1000;
            var offset = 0;
            var result = new List<FacebookUserProfile>().AsEnumerable();
            while (offset < partnerIds.Count())
            {
                var ids = partnerIds.Skip(offset).Take(limit);
                var items = await context.FacebookUserProfiles.Where(x => x.PartnerId.HasValue && ids.Contains(x.Partner.Id) && x.FbPageId == fbFageId).ToListAsync();

                result = result.Union(items);
                offset += limit;
            }

            return result;
        }


        private async Task<IEnumerable<Guid>> SearchPartnerIds(string graphXml, CatalogDbContext context)
        {
            if (string.IsNullOrEmpty(graphXml))
                return null;

            string logic = "and";
            var conditions = new List<object>();
            var conditionPartnerIds = new List<IList<Guid>>();
            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(graphXml)))
            {
                XmlReaderSettings settings = new XmlReaderSettings();
                settings.Async = true;
                using (XmlReader reader = XmlReader.Create(stream, settings))
                {
                    while (await reader.ReadAsync())
                    {
                        if (reader.NodeType == XmlNodeType.Element)
                        {
                            switch (reader.Name)
                            {
                                case "rule":
                                    {
                                        logic = reader.GetAttribute("logic");
                                        var subReader = reader.ReadSubtree();
                                        while (subReader.ReadToFollowing("condition"))
                                        {
                                            var type = subReader.GetAttribute("type");
                                            var name = subReader.GetAttribute("name");
                                            if (type == "categPartner")
                                            {
                                                var op = subReader.GetAttribute("op");
                                                Guid tagId;
                                                Guid.TryParse(subReader.GetAttribute("tagId"), out tagId);
                                                var cond = new PartnerCategoryCondition() { Name = name, Type = type, Op = op, TagId = tagId };
                                                conditions.Add(cond);
                                            }
                                            else if (type == "birthday")
                                            {
                                                int day = 0;
                                                int.TryParse(subReader.GetAttribute("day"), out day);
                                                var cond = new PartnerBirthdayCondition() { Name = name, Type = type, Day = day };
                                                conditions.Add(cond);
                                            }
                                            else if (type == "lastSaleOrder")
                                            {
                                                int day = 0;
                                                int.TryParse(subReader.GetAttribute("day"), out day);
                                                var cond = new LastSaleOrderCondition() { Name = name, Type = type, Day = day };
                                                conditions.Add(cond);
                                            }
                                            else if (type == "lastExamination")
                                            {
                                                int day = 0;
                                                int.TryParse(subReader.GetAttribute("day"), out day);
                                                var cond = new LastDotKhamDateCondition() { Name = name, Type = type, Day = day };
                                                conditions.Add(cond);
                                            }
                                            else if (type == "lastAppointment")
                                            {
                                                int day = 0;
                                                int.TryParse(subReader.GetAttribute("day"), out day);
                                                var cond = new LastAppointmentDateCondition() { Name = name, Type = type, Day = day };
                                                conditions.Add(cond);
                                            }
                                            //lần cuối sử dụng dịch vụ sau bao nhiêu ngày/tuần/tháng
                                            else if (type == "lastUsedServiceDate")
                                            {
                                                int intervalNumber = 0;
                                                int.TryParse(subReader.GetAttribute("intervalNumber"), out intervalNumber);
                                                var interval = subReader.GetAttribute("interval");

                                                var cond = new LastUsedServiceDate() { Type = type, NumberInterval = intervalNumber, Interval = interval };
                                                conditions.Add(cond);
                                            }
                                        }
                                        break;
                                    }
                                default:
                                    break;
                            }
                        }
                    }
                }
            }

            var query = context.Partners.Where(x => x.Customer && x.Active);
            //nếu không có condition nào thì gửi tất cả khách hàng
            if (conditions.Any())
            {
                foreach (var condition in conditions)
                {
                    var type = condition.GetType().GetProperty("Type").GetValue(condition, null);
                    switch (type)
                    {
                        case "categPartner":
                            {
                                var cond = (PartnerCategoryCondition)condition;
                                if (cond.Op == "not_contains")
                                {
                                    var searchPartnerIds = await query.Where(x => x.PartnerPartnerCategoryRels.Any(s => s.CategoryId == cond.TagId))
                                        .Select(x => x.Id).ToListAsync();
                                    conditionPartnerIds.Add(searchPartnerIds);
                                }
                                else
                                {
                                    var searchPartnerIds = await query.Where(x => !x.PartnerPartnerCategoryRels.Any(s => s.CategoryId == cond.TagId))
                                        .Select(x => x.Id).ToListAsync();
                                    conditionPartnerIds.Add(searchPartnerIds);
                                }
                                break;
                            }
                        case "birthday":
                            {
                                var today = DateTime.Today;
                                var cond = (PartnerBirthdayCondition)condition;
                                var date = today.AddDays(cond.Day);
                                var searchPartnerIds = await query.Where(x => x.BirthDay == date.Day && x.BirthMonth == date.Month)
                                    .Select(x => x.Id).ToListAsync();
                                conditionPartnerIds.Add(searchPartnerIds);
                                break;
                            }
                        case "lastSaleOrder":
                            {
                                var cond = (LastSaleOrderCondition)condition;
                                var date = DateTime.Now;
                                date = date.AddDays(-cond.Day);

                                var searchPartnerIds = await query.Where(x => x.SaleOrders.Where(s => s.State == "sale" || s.State == "done").Max(s => s.DateOrder) < date)
                                   .Select(x => x.Id).ToListAsync();

                                conditionPartnerIds.Add(searchPartnerIds);
                                break;
                            }
                        case "lastExamination":
                            {
                                var cond = (LastDotKhamDateCondition)condition;
                                var date = DateTime.Now;
                                date = date.AddDays(-cond.Day);

                                var searchPartnerIds = await query.Where(x => x.DotKhams.Max(s => s.Date) < date)
                                   .Select(x => x.Id).ToListAsync();

                                conditionPartnerIds.Add(searchPartnerIds);
                                break;
                            }
                        case "lastAppointment":
                            {
                                var cond = (LastAppointmentDateCondition)condition;
                                var date = DateTime.Today;
                                date = date.AddDays(-cond.Day);

                                var searchPartnerIds = await query.Where(x => x.Appointments.Max(s => s.Date.Date) == date)
                                   .Select(x => x.Id).ToListAsync();

                                conditionPartnerIds.Add(searchPartnerIds);
                                break;
                            }
                        case "lastUsedServiceDate":
                            {
                                var cond = (LastUsedServiceDate)condition;
                                var date = DateTime.Today;
                                var numberInterval = cond.NumberInterval;
                                var interval = cond.Interval;
                                if (interval == "month")
                                    date = date.AddMonths(-numberInterval);
                                else if (interval == "week")
                                    date = date.AddDays(-numberInterval * 7);
                                else
                                    date = date.AddDays(-numberInterval);

                                var states = new string[] { "sale", "done" };

                                var data = await context.SaleOrderLines.Where(x => x.OrderPartnerId.HasValue && states.Contains(x.Order.State))
                                    .GroupBy(x => x.OrderPartnerId.Value).Select(x => new {
                                        PartnerId = x.Key,
                                        LastDate = x.Max(s => s.Order.DateOrder)
                                    }).ToListAsync();

                                data = data.Where(x => x.LastDate.Date == date).ToList();
                                var searchPartnerIds = data.Select(x => x.PartnerId).ToList();
                                conditionPartnerIds.Add(searchPartnerIds);
                                break;
                            }
                        default:
                            break;
                    }
                }

                IEnumerable<Guid> result = null;
                foreach (var item in conditionPartnerIds)
                {
                    if (result == null)
                    {
                        result = item;
                        continue;
                    }

                    if (logic == "or")
                        result = result.Union(item);
                    else
                        result = result.Intersect(item);
                }

                return result;
            }
            else
            {
                return await query.Select(x => x.Id).ToListAsync();
            }
        }

        private string GetCampaignContent(string graphXml)
        {
            if (string.IsNullOrEmpty(graphXml))
                return string.Empty;
            XmlSerializer serializer = new XmlSerializer(typeof(MxGraphModel));
            MemoryStream memStream = new MemoryStream(Encoding.UTF8.GetBytes(graphXml));
            MxGraphModel resultingMessage = (MxGraphModel)serializer.Deserialize(memStream);
            var sequence = resultingMessage.Root != null ? resultingMessage.Root.Sequence : null;
            if (sequence != null)
                return sequence.Content;
            return string.Empty;
        }

        private string GetCampaignCouponId(string graphXml)
        {
            if (string.IsNullOrEmpty(graphXml))
                return string.Empty;
            XmlSerializer serializer = new XmlSerializer(typeof(MxGraphModel));
            MemoryStream memStream = new MemoryStream(Encoding.UTF8.GetBytes(graphXml));
            MxGraphModel resultingMessage = (MxGraphModel)serializer.Deserialize(memStream);
            var sequence = resultingMessage.Root != null ? resultingMessage.Root.Sequence : null;
            if (sequence != null)
                return sequence.CouponProgramId;
            return string.Empty;
        }

        private DateTime GetScheduleDateCampaign(TCareCampaign campaign)
        {
            //xác định thời điểm gửi tin của chiến dịch
            var date = DateTime.Now;
            if (campaign.SheduleStart.HasValue)
                date = date.AddHours(campaign.SheduleStart.Value.Hour).AddMinutes(campaign.SheduleStart.Value.Minute);
            return date;
        }

        private async Task CreateMessages(CatalogDbContext context, TCareMessaging messaging, IEnumerable<Guid> partnerIds)
        {
            //Tìm profiles sẽ gửi cho list partnerIds
            var profiles = await SearchPartnerProfiles(partnerIds, messaging.FacebookPageId, context);
            var profileDict = profiles.ToDictionary(x => x.PartnerId, x => x);

            var sendPartnerIds = profiles.Select(x => x.PartnerId).ToList();
            var partners = await GetPartnersData(sendPartnerIds, context);
            var partnerDict = partners.ToDictionary(x => x.Id, x => x);

            var channel = await context.FacebookPages.FindAsync(messaging.FacebookPageId);

            var messages = new List<TCareMessage>();
            foreach (var profile in profiles)
            {
                if (!partnerDict.ContainsKey(profile.PartnerId))
                    continue;

                var partner = partnerDict[profile.PartnerId];

                var messageContent = PersonalizedContent(partner, channel, messaging.Content);

                var message = new TCareMessage()
                {
                    ProfilePartnerId = profile.Id,
                    ChannelSocicalId = messaging.FacebookPageId,
                    CampaignId = messaging.TCareCampaignId,
                    PartnerId = partner.Id,
                    MessageContent = messageContent,
                    TCareMessagingId = messaging.Id,
                    State = "waiting",
                    ScheduledDate = messaging.ScheduleDate,
                };

                messages.Add(message);
            }

            await context.AddRangeAsync(messages);
            await context.SaveChangesAsync();
        }

        private async Task<IEnumerable<TCareCampaignJobPartnerProfile>> SearchPartnerProfiles(IEnumerable<Guid> partnerIds, Guid? fbFageId, CatalogDbContext context)
        {
            var limit = 1000;
            var offset = 0;
            var result = new List<TCareCampaignJobPartnerProfile>();
            while (offset < partnerIds.Count())
            {
                var ids = partnerIds.Skip(offset).Take(limit);
                var items = await context.FacebookUserProfiles.Where(x => x.PartnerId.HasValue && ids.Contains(x.Partner.Id) && x.FbPageId == fbFageId)
                .Select(x => new TCareCampaignJobPartnerProfile
                {
                    Id = x.Id,
                    PartnerId = x.PartnerId.Value
                }).ToListAsync();
                result.AddRange(items);

                offset += limit;
            }

            return result;
        }

        private async Task<IEnumerable<TCareCampaignJobPartnerInfo>> GetPartnersData(IEnumerable<Guid> partnerIds, CatalogDbContext context)
        {
            var limit = 1000;
            var offset = 0;
            var result = new List<TCareCampaignJobPartnerInfo>();
            while (offset < partnerIds.Count())
            {
                var ids = partnerIds.Skip(offset).Take(limit);
                var items = await context.Partners.Where(x => ids.Contains(x.Id))
                .Select(x => new TCareCampaignJobPartnerInfo
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

        private string PersonalizedContent(TCareCampaignJobPartnerInfo partner, FacebookPage channelSocial, string content)
        {
            var messageContent = content.Replace("@ten_khach_hang", partner.Name.Split(' ').Last())
                .Replace("@fullname_khach_hang", partner.Name)
                .Replace("@ten_page", channelSocial.PageName)
                .Replace("@danh_xung_khach_hang", partner.Title);
            return messageContent;
        }

        public async Task ProcessTag(Guid id, string psid, string db, string type)
        {
            await using var context = DbContextHelper.GetCatalogDbContext(db, _configuration);
            var campaign = await context.TCareCampaigns.Where(x => x.Id == id).FirstOrDefaultAsync();
            if (campaign == null)
                return;

            var profile = await context.FacebookUserProfiles.Where(x => x.PSID == psid && x.FbPageId == campaign.FacebookPageId).FirstOrDefaultAsync();
            if (profile == null || !profile.PartnerId.HasValue)
                return;

            XmlSerializer serializer = new XmlSerializer(typeof(MxGraphModel));
            MemoryStream memStream = new MemoryStream(Encoding.UTF8.GetBytes(campaign.GraphXml));
            MxGraphModel CampaignXML = (MxGraphModel)serializer.Deserialize(memStream);

            if (CampaignXML.Root.AddTag.Any())
            {
                var readAddTags = CampaignXML.Root.AddTag.Where(x => x.MxCell.Style == type).FirstOrDefault();

                if (readAddTags != null && readAddTags.Tag.Count > 0)
                {
                    var tagIds = readAddTags.Tag.Select(x => x.Id);
                    var tags = await context.PartnerCategories.Where(x => tagIds.Contains(x.Id)).ToListAsync(); //tránh trường hợp tag đã bị xóa nhưng tồn tại id trong xml
                    var partner = await context.Partners.Where(x => x.Id == profile.PartnerId).Include(x => x.PartnerPartnerCategoryRels).FirstOrDefaultAsync();
                    if (partner != null)
                    {
                        var tagsToAdd = tags.Where(x => !partner.PartnerPartnerCategoryRels.Any(s => s.CategoryId == x.Id));
                        foreach (var tag in tagsToAdd)
                        {
                            partner.PartnerPartnerCategoryRels.Add(new PartnerPartnerCategoryRel
                            {
                                CategoryId = tag.Id,
                            });
                        }

                        await context.SaveChangesAsync();
                    }
                }
            }
        }
    }

    public class TCareCampaignJobPartnerProfile
    {
        public Guid Id { get; set; }
        public Guid PartnerId { get; set; }
    }

    public class TCareCampaignJobPartnerInfo
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Title { get; set; }
    }
}
