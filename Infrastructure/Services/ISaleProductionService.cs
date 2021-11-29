using ApplicationCore.Entities;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public interface ISaleProductionService : IBaseService<SaleProduction>
    {
        Task CompareSaleProduction(IEnumerable<SaleProduction> saleProductions);
        Task Unlink(IEnumerable<Guid> ids);

    }
}
