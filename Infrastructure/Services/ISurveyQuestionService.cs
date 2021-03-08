using ApplicationCore.Entities;
using ApplicationCore.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Web.Models.ContentEditing;

namespace Infrastructure.Services
{
    public interface ISurveyQuestionService: IBaseService<SurveyQuestion>
    {
        Task<PagedResult2<SurveyQuestionBasic>> GetPagedResultAsync(SurveyQuestionPaged val);
        Task ActionActive(ActionActivePar val);
    }
}
