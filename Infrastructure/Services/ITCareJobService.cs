using ApplicationCore.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public interface ITCareJobService
    {
        void Run(string db, Guid campaignId);

    }
}
