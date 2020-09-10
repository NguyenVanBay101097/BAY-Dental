using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class TimeKeeperSync
    {
        public IList<ChamCongImportResponse> ErrorDatas { get; set; } = new List<ChamCongImportResponse>();
    }
}
