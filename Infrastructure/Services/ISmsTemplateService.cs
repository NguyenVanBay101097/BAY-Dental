using ApplicationCore.Entities;
using ApplicationCore.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Web.Models.ContentEditing;

namespace Infrastructure.Services
{
    public interface ISmsTemplateService : IBaseService<SmsTemplate>
    {
        Task<PagedResult2<SmsTemplateBasic>> GetPaged(SmsTemplatePaged val);
        Task<IEnumerable<SmsTemplateBasic>> GetTemplateAutocomplete(string filter);
    }
}
