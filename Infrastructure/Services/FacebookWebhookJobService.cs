using ApplicationCore.Entities;
using Dapper;
using Infrastructure.Data;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Umbraco.Web.Models.ContentEditing;

namespace Infrastructure.Services
{
    public class FacebookWebhookJobService: IFacebookWebhookJobService
    {
        private readonly ConnectionStrings _connectionStrings;
        public FacebookWebhookJobService(IOptions<ConnectionStrings> connectionStrings)
        {
            _connectionStrings = connectionStrings?.Value;
        }

        public async Task ProcessRead(string db, DateTime watermark, string sender_id, string recipient_id)
        {
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(_connectionStrings.CatalogConnection);
            builder["Database"] = $"TMTDentalCatalogDb__{db}";
            if (db == "localhost")
                builder["Database"] = "TMTDentalCatalogDb";

            using (var conn = new SqlConnection(builder.ConnectionString))
            {
                var traces = await conn.QueryAsync<dynamic>("select trace.Id, trace.TCareCampaignId, trace.PartnerId from TCareMessingTraces trace left join FacebookPages p on trace.ChannelSocialId = p.Id where trace.MessageId is not null and trace.Opened is null and trace.Sent <= @date and trace.PSID = @psid and p.PageId = @pageId", new { psid = sender_id, pageId = recipient_id, date = watermark });
                var prams = traces.Select(x => new { opened = watermark, id = x.Id }).ToArray();
                await conn.ExecuteAsync("update TCareMessingTraces set Opened=@opened where @Id=id;", prams);

                var tmp = traces.Select(x => new { PartnerId = x.PartnerId, TCareCampaignId = x.TCareCampaignId }).Distinct().ToList();
                foreach (var item in tmp)
                {
                    if (item.PartnerId is null || item.TCareCampaignId is null)
                        continue;
                    var campaign = conn.Query<TCareCampaign>("SELECT * FROM TCareCampaigns WHERE Id = @id", new { id = item.TCareCampaignId }).FirstOrDefault();
                  
                    XmlSerializer serializer = new XmlSerializer(typeof(MxGraphModel));
                    MemoryStream memStream = new MemoryStream(Encoding.UTF8.GetBytes(campaign.GraphXml));
                    MxGraphModel CampaignXML = (MxGraphModel)serializer.Deserialize(memStream);
                    if (CampaignXML.Root.AddTag.Any())
                    {
                        var addtags = CampaignXML.Root.AddTag.Where(x => x.MxCell.Style == "read").FirstOrDefault();
                        if (addtags != null)
                        {
                            var tags = await conn.QueryAsync<PartnerPartnerCategoryRel>("SELECT * FROM PartnerPartnerCategoryRel WHERE PartnerId = @id", new { id = item.PartnerId });
                            var tags_to_add = new List<PartnerPartnerCategoryRel>();
                            foreach (var categ in addtags.Tag)
                            {
                                if (tags.Any(x => x.CategoryId == categ.Id))
                                    continue;
                                tags_to_add.Add(new PartnerPartnerCategoryRel
                                {
                                    CategoryId = categ.Id,
                                    PartnerId = item.PartnerId
                                });
                            }

                            if (tags_to_add.Any())
                                await conn.ExecuteAsync("insert into PartnerPartnerCategoryRel(CategoryId,PartnerId) values (@CategoryId,@PartnerId);", tags_to_add.ToArray());
                        }
                    }

                }
            }
        }

        public async Task ProcessDelivery(string db, DateTime watermark, string sender_id, string recipient_id)
        {
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(_connectionStrings.CatalogConnection);
            builder["Database"] = $"TMTDentalCatalogDb__{db}";
            if (db == "localhost")
                builder["Database"] = "TMTDentalCatalogDb";

            using (var conn = new SqlConnection(builder.ConnectionString))
            {
                var traces = await conn.QueryAsync<dynamic>("select trace.Id, trace.TCareCampaignId, trace.PartnerId from TCareMessingTraces trace left join FacebookPages p on trace.ChannelSocialId = p.Id where trace.MessageId is not null and trace.Delivery is null and trace.Sent <= @date and trace.PSID = @psid and p.PageId = @pageId", new { psid = sender_id, pageId = recipient_id, date = watermark });
                var prams = traces.Select(x => new { delivery = watermark, id = x.Id }).ToArray();
                await conn.ExecuteAsync("update TCareMessingTraces set Delivery=@delivery where @Id=id;", prams);

                var tmp = traces.Select(x => new { PartnerId = x.PartnerId, TCareCampaignId = x.TCareCampaignId }).Distinct().ToList();
                foreach (var item in tmp)
                {
                    if (item.PartnerId is null || item.TCareCampaignId is null)
                        continue;
                    var campaign = conn.Query<TCareCampaign>("SELECT * FROM TCareCampaigns WHERE Id = @id", new { id = item.TCareCampaignId }).FirstOrDefault();

                    XmlSerializer serializer = new XmlSerializer(typeof(MxGraphModel));
                    MemoryStream memStream = new MemoryStream(Encoding.UTF8.GetBytes(campaign.GraphXml));
                    MxGraphModel CampaignXML = (MxGraphModel)serializer.Deserialize(memStream);
                    if (CampaignXML.Root.AddTag.Any())
                    {
                        var addtags = CampaignXML.Root.AddTag.Where(x => x.MxCell.Style == "unread").FirstOrDefault();
                        if (addtags != null)
                        {
                            var tags = await conn.QueryAsync<PartnerPartnerCategoryRel>("SELECT * FROM PartnerPartnerCategoryRel WHERE PartnerId = @id", new { id = item.PartnerId });
                            var tags_to_add = new List<PartnerPartnerCategoryRel>();
                            foreach (var categ in addtags.Tag)
                            {
                                if (tags.Any(x => x.CategoryId == categ.Id))
                                    continue;
                                tags_to_add.Add(new PartnerPartnerCategoryRel
                                {
                                    CategoryId = categ.Id,
                                    PartnerId = item.PartnerId
                                });
                            }

                            if (tags_to_add.Any())
                                await conn.ExecuteAsync("insert into PartnerPartnerCategoryRel(CategoryId,PartnerId) values (@CategoryId,@PartnerId);", tags_to_add.ToArray());
                        }
                    }

                }
            }
        }
    }
}
