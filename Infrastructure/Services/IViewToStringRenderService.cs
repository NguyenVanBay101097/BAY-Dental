using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Web.Models.ContentEditing;

namespace Infrastructure.Services
{
    public interface IViewToStringRenderService
    {
        Task<string> RenderViewAsync<TModel>(string viewName, TModel model , ConfigPrintDisplay viewdata);
    }
}
