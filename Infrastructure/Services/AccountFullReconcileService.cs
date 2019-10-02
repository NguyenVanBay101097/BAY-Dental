using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class AccountFullReconcileService : BaseService<AccountFullReconcile>, IAccountFullReconcileService
    {

        public AccountFullReconcileService(IAsyncRepository<AccountFullReconcile> repository, IHttpContextAccessor httpContextAccessor)
        : base(repository, httpContextAccessor)
        {
        }

        public override async Task<AccountFullReconcile> CreateAsync(AccountFullReconcile entity)
        {
            var seqObj = GetService<IIRSequenceService>();
            entity.Name = await seqObj.NextByCode("account.reconcile");
            return await base.CreateAsync(entity);
        }
    }
}
