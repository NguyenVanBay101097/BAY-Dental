using ApplicationCore.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public interface IIRModelDataService: IBaseService<IRModelData>
    {
        Task<IRModelData> GetObjectReference(string reference);
        Task<T> GetRef<T>(string reference) where T : class;
    }
}
