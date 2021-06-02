using ApplicationCore.Entities;
using Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.HangfireJobService
{
    public interface ISmsJobService
    {
        Task RunAppointmentAutomatic(string db, Guid configId);
        Task RunBirthdayAutomatic(string db, Guid configId);
        Task RunCareAfterOrderAutomatic(string db, Guid configId);
    }
}
