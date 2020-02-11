using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Services
{
    public class IRModelFieldService : BaseService<IRModelField>, IIRModelFieldService
    {
        public IRModelFieldService(IAsyncRepository<IRModelField> repository, IHttpContextAccessor httpContextAccessor)
            : base(repository, httpContextAccessor)
        {
        }
    }
}
