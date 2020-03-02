using ApplicationCore.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public interface IFacebookConfigPageService: IBaseService<FacebookConfigPage>
    {
        Task SyncAllUserCanChatWithPage(Guid id);
        Task SyncData(Guid id);
    }
}
