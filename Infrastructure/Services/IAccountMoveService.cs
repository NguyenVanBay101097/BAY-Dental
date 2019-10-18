using ApplicationCore.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public interface IAccountMoveService : IBaseService<AccountMove>
    {
        Task Post(IEnumerable<AccountMove> moves, AccountInvoice invoice = null);
        Task ButtonCancel(IEnumerable<AccountMove> self);
        Task ButtonCancel(IEnumerable<Guid> ids);
        bool _CheckLockDate(IEnumerable<AccountMove> self);
        Task Unlink(IEnumerable<AccountMove> self);
        Task Unlink(IEnumerable<Guid> ids);
        Task Write(IEnumerable<AccountMove> self);
    }
}
