using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Services
{
    public interface IMarketingCampaignActivityJobService
    {
        void RunActivity(string db, Guid activityId);
    }
}
