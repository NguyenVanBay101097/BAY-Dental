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
        Task RunJob(string db, Guid configId);
        Task RunAppointmentAutomatic(CatalogDbContext context, SmsConfig config);
        Task RunBirthdayAutomatic(CatalogDbContext context, SmsConfig config);
        Task RunThanksCustomerAutomatic(CatalogDbContext context, SmsConfig config);
        Task RunCareAfterOrderAutomatic(CatalogDbContext context, SmsConfig config);
    }
}
