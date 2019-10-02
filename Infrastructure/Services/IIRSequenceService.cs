using ApplicationCore.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public interface IIRSequenceService: IBaseService<IRSequence>
    {
        Task<string> NextByCode(string v);
        Task<string> NextById(Guid id);
    }
}
