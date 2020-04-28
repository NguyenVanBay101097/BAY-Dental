using ApplicationCore.Entities;
using ApplicationCore.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public interface IIRModelService: IBaseService<IRModel>
    {
        Task<PagedResult2<IRModel>> GetPagedAsync(int offset = 0, int limit = 10, string filter = "");
        Task InsertSampleData();
        Task<IDictionary<string, IRModel>> InsertIfNotExist(IDictionary<string, IRModel> dict);
    }
}
