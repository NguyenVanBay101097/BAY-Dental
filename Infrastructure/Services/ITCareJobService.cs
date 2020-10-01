using ApplicationCore.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public interface ITCareJobService
    {
        Task Run(string db, Guid campaignId);

    }
}
