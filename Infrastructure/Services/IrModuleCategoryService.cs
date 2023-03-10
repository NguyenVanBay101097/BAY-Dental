using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Services
{
    public class IrModuleCategoryService : BaseService<IrModuleCategory>, IIrModuleCategoryService
    {
        public IrModuleCategoryService(IAsyncRepository<IrModuleCategory> repository, IHttpContextAccessor httpContextAccessor) 
            : base(repository, httpContextAccessor)
        {
        }
    }
}
