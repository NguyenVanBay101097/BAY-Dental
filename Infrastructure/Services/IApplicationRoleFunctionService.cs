using ApplicationCore.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public interface IApplicationRoleFunctionService: IBaseService<ApplicationRoleFunction>
    {
        Task<bool> HasAccess(List<string> functions, bool raiseException = true);
    }
}
