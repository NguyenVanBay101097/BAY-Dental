using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Services
{
    public class UoMCategoryService : BaseService<UoMCategory>, IUoMCategoryService
    {

        public UoMCategoryService(IAsyncRepository<UoMCategory> repository, IHttpContextAccessor httpContextAccessor)
        : base(repository, httpContextAccessor)
        {
        }
    }
}
