using ApplicationCore.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public interface IIrConfigParameterService: IBaseService<IrConfigParameter>
    {
        Task<string> SetParam(string key, string value);
        Task<string> GetParam(string key);
    }
}
