using ApplicationCore.Entities;
using Dapper;
using Facebook.ApiClient.ApiEngine;
using Facebook.ApiClient.Constants;
using Infrastructure.Data;
using Microsoft.Extensions.Options;
using MyERP.Utilities;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Umbraco.Web.Models.ContentEditing;
using ZaloDotNetSDK;
using ZaloDotNetSDK.oa;

namespace Infrastructure.Services
{
    public class FacebookWebhookJobService : IFacebookWebhookJobService
    {
        private readonly ConnectionStrings _connectionStrings;
        public FacebookWebhookJobService(IOptions<ConnectionStrings> connectionStrings)
        {
            _connectionStrings = connectionStrings?.Value;
        }

        public async Task ProcessRead(string db, DateTime watermark, string psid, string page_id)
        {
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(_connectionStrings.CatalogConnection);
            if (db != "localhost")
                builder["Database"] = $"TMTDentalCatalogDb__{db}";

            using (var conn = new SqlConnection(builder.ConnectionString))
            {
                var traces = await conn.QueryAsync<dynamic>("select trace.Id, trace.TCareCampaignId, trace.PartnerId from TCareMessingTraces trace left join FacebookPages p on trace.ChannelSocialId = p.Id where trace.MessageId is not null and trace.Opened is null and trace.Sent <= @date and trace.PSID = @psid and p.PageId = @pageId", new { psid = psid, pageId = page_id, date = watermark });
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

        public async Task ProcessDelivery(string db, DateTime watermark, string psid, string page_id )
        {
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(_connectionStrings.CatalogConnection);
            if (db != "localhost")
                builder["Database"] = $"TMTDentalCatalogDb__{db}";

            using (var conn = new SqlConnection(builder.ConnectionString))
            {
                var traces = await conn.QueryAsync<dynamic>("select trace.Id, trace.TCareCampaignId, trace.PartnerId from TCareMessingTraces trace left join FacebookPages p on trace.ChannelSocialId = p.Id where trace.MessageId is not null and trace.Delivery is null and trace.Sent <= @date and trace.PSID = @psid and p.PageId = @pageId", new { psid = psid, pageId = page_id, date = watermark });
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

        public async Task ProcessAddUserProfile(string db, string psid, string page_id)
        {
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(_connectionStrings.CatalogConnection);
            if (db != "localhost")
                builder["Database"] = $"TMTDentalCatalogDb__{db}";

            using (var conn = new SqlConnection(builder.ConnectionString))
            {
                var now = DateTime.Now;              
                var page = conn.Query<FacebookPage>("select * from FacebookPages where PageId = @PageId ", new { PageId = page_id }).FirstOrDefault();
                var profile = conn.Query<FacebookUserProfile>("select * from FacebookUserProfiles where PSID = @PSID AND FbPageId = @FbPageId", new { PSID = psid, FbPageId = page.Id }).FirstOrDefault();
                if(page.Type == "facebook")
                {
                    if (profile == null)
                    {
                        var apiClient = new ApiClient(page.PageAccesstoken, FacebookApiVersions.V6_0);
                        var getRequestUrl = $"{psid}?fields=id,name,first_name,last_name,profile_pic";
                        var getRequest = (GetRequest)ApiRequest.Create(ApiRequest.RequestType.Get, getRequestUrl, apiClient, false);
                        var response = (await getRequest.ExecuteAsync<ApiUserProfileResponse>());
                        if (!response.GetExceptions().Any())
                        {
                            var rs = response.GetResult();
                            await conn.ExecuteAsync("insert into FacebookUserProfiles(Id,DateCreated,LastUpdated,Name,FirstName,LastName,Gender,PSID,FbPageId,Avatar) values (@Id,@DateCreated,@LastUpdated,@Name,@FirstName,@LastName,@Gender,@PSID,@FbPageId,@Avatar);", new { Id = GuidComb.GenerateComb(), DateCreated = now, LastUpdated = now, Name = rs.Name, FirstName = rs.FirstName, LastName = rs.LastName, Gender = rs.Gender, PSID = rs.PSId, FbPageId = page.Id, Avatar = rs.ProfilePic });
                        }
                    }
                    return;
                }
                else if (page.Type == "zalo")
                {
                    if (profile == null)
                    {
                        var zaloClient = new ZaloClient(page.PageAccesstoken);
                        var res = zaloClient.getProfileOfFollower(psid).ToObject<GetProfileOfFollowerResponse>();
                        await conn.ExecuteAsync("insert into FacebookUserProfiles(Id,DateCreated,LastUpdated,Name,Gender,PSID,FbPageId,Avatar) values (@Id,@DateCreated,@LastUpdated,@Name,@Gender,@PSID,@FbPageId,@Avatar);", new { Id = GuidComb.GenerateComb(), DateCreated = now, LastUpdated = now, Name = res.data.display_name, Gender = res.data.user_gender, PSID = res.data.user_id.ToString(), FbPageId = page.Id, Avatar = res.data.avatar });
                    }
                    
                }
                

               
            }
        }
    }
}
