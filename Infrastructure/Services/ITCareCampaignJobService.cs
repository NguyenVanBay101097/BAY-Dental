using ApplicationCore.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public interface ITCareCampaignJobService
    {
        Task Run(string db, IEnumerable<TCareCampaign> campaignId = null);
    }
}
