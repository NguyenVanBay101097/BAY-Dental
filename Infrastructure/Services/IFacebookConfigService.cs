using ApplicationCore.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Web.Models.ContentEditing;

namespace Infrastructure.Services
{
    public interface IFacebookConfigService: IBaseService<FacebookConfig>
    {
        Task<FacebookConfig> CreateConfig(FacebookConfigSave val);
        Task<FacebookConfig> CreateConfigConnect(FacebookConfigConnectSave val);
    }
}
