using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class UserChangeCurrentCompanyVM
    {
        public CompanyBasic CurrentCompany { get; set; }

        public IEnumerable<CompanyBasic> Companies { get; set; }
    }
}
