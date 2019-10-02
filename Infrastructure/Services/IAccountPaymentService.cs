using ApplicationCore.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public interface IAccountPaymentService: IBaseService<AccountPayment>
    {
        Task Post(IEnumerable<AccountPayment> payments);
    }
}
