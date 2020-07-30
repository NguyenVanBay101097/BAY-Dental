using ApplicationCore.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public interface IFacebookConnectPageService: IBaseService<FacebookConnectPage>
    {
        Task AddConnect(IEnumerable<Guid> ids);
        Task RemoveConnect(IEnumerable<Guid> ids);
     
    }
}
