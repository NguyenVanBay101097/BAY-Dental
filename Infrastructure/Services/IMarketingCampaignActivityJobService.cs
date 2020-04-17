using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public interface IMarketingCampaignActivityJobService
    {
        Task RunActivity(string db, Guid activityId);
        //Task CreateRunActivetyContinueJobAsync(string db, Guid activityId, DateTime dateFrom, DateTime dateTo);
    }
}
