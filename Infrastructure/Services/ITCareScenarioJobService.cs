using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public interface ITCareScenarioJobService
    {
        Task Run(string db, IEnumerable<Guid> ids);
    }
}
