using ApplicationCore.Entities;
using ApplicationCore.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Web.Models.ContentEditing;

namespace Infrastructure.Services
{
    public interface ISurveyCallContentService : IBaseService<SurveyCallContent>
    {
        Task<PagedResult2<SurveyCallContentBasic>> GetPagedResultAsync(SurveyCallContentPaged val);
        Task<SurveyCallContentDisplay> GetDisplay(Guid id);
        Task<SurveyCallContent> CreateSurveyCallContent(SurveyCallContentSave val);
        Task UpdateSurveyCallContent(Guid id, SurveyCallContentSave val);
    }
}
