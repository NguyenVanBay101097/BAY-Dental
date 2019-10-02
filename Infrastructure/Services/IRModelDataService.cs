using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class IRModelDataService : BaseService<IRModelData>, IIRModelDataService
    {

        public IRModelDataService(IAsyncRepository<IRModelData> repository, IHttpContextAccessor httpContextAccessor)
        : base(repository, httpContextAccessor)
        {
        }

        public async Task<IRModelData> GetObjectReference(string reference)
        {
            if (string.IsNullOrEmpty(reference))
                return null;

            var tmp = reference.Split('.');
            var module = tmp[0];
            var name = tmp[1];
            return await SearchQuery(x => x.Name == name && x.Module == module).FirstOrDefaultAsync();
        }
    }
}
