using ApplicationCore.Entities;
using Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public interface ISmsSendMessageService
    {
        Task CreateSmsSms(CatalogDbContext context, SmsComposer composer, IEnumerable<Guid> ids, Guid companyId);
    }
}
