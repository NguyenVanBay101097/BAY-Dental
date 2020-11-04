using ApplicationCore.Entities;
using ApplicationCore.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Web.Models.ContentEditing;

namespace Infrastructure.Services
{
    public interface ITCareMessageTemplateService: IBaseService<TCareMessageTemplate>
    {
        Task<PagedResult2<TCareMessageTemplateBasic>> GetPaged(TCareMessageTemplatePaged val);
        Task<TCareMessageTemplateDisplay> GetDisplay(Guid id);
    }
}
