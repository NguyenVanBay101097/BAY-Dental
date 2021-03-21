using ApplicationCore.Entities;
using ApplicationCore.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Web.Models.ContentEditing;

namespace Infrastructure.Services
{
    public interface ISurveyAssignmentService: IBaseService<SurveyAssignment>
    {
        Task<PagedResult2<SurveyAssignmentGridItem>> GetPagedResultAsync(SurveyAssignmentPaged val);
        Task<SurveyAssignmentDisplay> GetDisplay(Guid id);

        Task ActionContact(IEnumerable<Guid> ids);

        Task ActionCancel(IEnumerable<Guid> ids);

        //Task ActionDone(AssignmentActionDone val);

        Task<PagedResult2<SurveyAssignmentDefaultGet>> DefaultGetList(SurveyAssignmentDefaultGetPar val);
        Task<IEnumerable<SurveyAssignmentGetSummary>> GetSummary(SurveyAssignmentGetSummaryFilter val);

        Task AddIrDataForSurvey();
    }
}
