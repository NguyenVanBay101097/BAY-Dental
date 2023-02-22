using ApplicationCore.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public interface IDotKhamLineOperationService: IBaseService<DotKhamLineOperation>
    {
        Task MarkDone(IEnumerable<Guid> ids);
        Task StartOperation(Guid id);
    }
}
