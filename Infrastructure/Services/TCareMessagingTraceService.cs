using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using ApplicationCore.Models.ContentEditing;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Umbraco.Web.Models.ContentEditing;

namespace Infrastructure.Services
{
    public class TCareMessagingTraceService : BaseService<TCareMessagingTrace>, ITCareMessagingTraceService
    {
        private readonly IMapper _mapper;
        public TCareMessagingTraceService(IAsyncRepository<TCareMessagingTrace> repository, IHttpContextAccessor httpContextAccessor , IMapper mapper ) 
            : base(repository, httpContextAccessor)
        {
            _mapper = mapper;
        }

        public async Task AddTagWebhook(IEnumerable<TCareMessagingTrace> traces , string type )
        {

            var objPartner = GetService<IPartnerService>();
            var objCampaign = GetService<ITCareCampaignService>();
            foreach (var trace in traces)
            {
               
                var partner = await objPartner.GetPartnerForDisplayAsync(trace.PartnerId.Value);
                var campaign = await objCampaign.SearchQuery(x => x.Id == trace.TCareCampaignId).FirstOrDefaultAsync();
                XmlSerializer serializer = new XmlSerializer(typeof(MxGraphModel));
                MemoryStream memStream = new MemoryStream(Encoding.UTF8.GetBytes(campaign.GraphXml));
                MxGraphModel CampaignXML = (MxGraphModel)serializer.Deserialize(memStream);
                if (CampaignXML.Root.AddTag.Any())
                {
                    var addtags = CampaignXML.Root.AddTag.Where(x => x.MxCell.Style == type).FirstOrDefault();
                    if (addtags != null)
                    {
                        var toRemove = partner.PartnerPartnerCategoryRels.Where(x => !addtags.Tag.Any(s => s.Id == x.CategoryId)).ToList();
                        foreach (var categ in toRemove)
                        {
                            partner.PartnerPartnerCategoryRels.Remove(categ);
                        }
                        foreach (var categ in addtags.Tag)
                        {
                            if (partner.PartnerPartnerCategoryRels.Any(x => x.CategoryId == categ.Id))
                                continue;
                            partner.PartnerPartnerCategoryRels.Add(new PartnerPartnerCategoryRel
                            {
                                CategoryId = categ.Id
                            });

                        }
                        await objPartner.UpdateAsync(partner);
                    }
                }
               
            }
        }
                                           
    }
}
