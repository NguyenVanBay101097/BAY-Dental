using ApplicationCore.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Web.Models.ContentEditing;

namespace Infrastructure.Services
{
    public interface ISurveyUserInputService : IBaseService<SurveyUserInput>
    {
        Task<SurveyUserInputDisplay> GetDisplay(Guid id);

        Task<SurveyUserInputDisplay> DefaultGet();

        Task<SurveyUserInput> CreateUserInput(SurveyUserInputSave val);

        Task UpdateUserInput(Guid id, SurveyUserInputSave val);

        Task Unlink(Guid id);
    }
}
