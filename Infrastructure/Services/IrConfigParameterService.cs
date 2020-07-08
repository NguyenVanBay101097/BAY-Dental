using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class IrConfigParameterService : BaseService<IrConfigParameter>, IIrConfigParameterService
    {
        public IrConfigParameterService(IAsyncRepository<IrConfigParameter> repository, IHttpContextAccessor httpContextAccessor)
            : base(repository, httpContextAccessor)
        {
        }

        public async Task<string> SetParam(string key, string value)
        {
            var param = await SearchQuery(x => x.Key == key).FirstOrDefaultAsync();
            if (param != null)
            {
                var old = param.Value;
                if (!string.IsNullOrEmpty(value))
                {
                    if (value != old)
                    {
                        param.Value = value;
                        await UpdateAsync(param);
                    }
                }
                else
                    await DeleteAsync(param);
                return old;
            }
            else
            {
                if (!string.IsNullOrEmpty(value))
                    await CreateAsync(new IrConfigParameter { Key = key, Value = value });
                return null;
            }
        }

        public async Task<string> GetParam(string key)
        {
            var value = await SearchQuery(x => x.Key == key).Select(x => x.Value).FirstOrDefaultAsync();
            return value;
        }
    }
}
