using ApplicationCore.Entities;
using ApplicationCore.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Web.Models.ContentEditing;

namespace Infrastructure.Services
{
    public interface ISurveyTagService : IBaseService<SurveyTag>
    {
        Task<PagedResult2<SurveyTagBasic>> GetPagedResultAsync(SurveyTagPaged val);
    }
}
