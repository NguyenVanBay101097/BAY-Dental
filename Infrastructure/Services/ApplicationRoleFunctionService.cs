using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class ApplicationRoleFunctionService : BaseService<ApplicationRoleFunction>, IApplicationRoleFunctionService
    {

        public ApplicationRoleFunctionService(IAsyncRepository<ApplicationRoleFunction> repository, IHttpContextAccessor httpContextAccessor)
        : base(repository, httpContextAccessor)
        {
        }

        public async Task<bool> Check(string function, bool raiseException = true)
        {
            //var functionP = new SqlParameter("function", function);
            //var userP = new SqlParameter("userId", UserId);

            //var funcs = await SearchQuery().FromSql("select a.Id,a.CreatedById,a.WriteById,a.DateCreated,a.LastUpdated,a.Func,a.RoleId from ApplicationRoleFunctions a " +
            //    "INNER JOIN AspNetUserRoles gu ON gu.RoleId=a.RoleId " +
            //    "WHERE a.Func=@function " +
            //    "AND gu.UserId=@userId", functionP, userP).ToListAsync();
            //if (funcs.Count == 0 && raiseException)
            //{
            //    throw new Exception("Bạn không có quyền thực hiện thao tác này.");
            //}
            //return funcs.Count > 0;
            return true;
        }
    }
}
