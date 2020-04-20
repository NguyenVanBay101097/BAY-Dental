using ApplicationCore.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public interface IServiceCardCardService: IBaseService<ServiceCardCard>
    {
        Task ActionActive(IEnumerable<ServiceCardCard> self);
    }
}
