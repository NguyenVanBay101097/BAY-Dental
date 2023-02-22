using ApplicationCore.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public interface IAccountPartialReconcileService: IBaseService<AccountPartialReconcile>
    {
        Task Unlink(IEnumerable<Guid> ids);
    }
}
