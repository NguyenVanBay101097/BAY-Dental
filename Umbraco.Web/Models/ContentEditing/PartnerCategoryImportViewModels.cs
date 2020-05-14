using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class PartnerCategoryRowExcel
    {
        public string Name { get; set; }
    }

    public class PartnerCategoryImportExcelViewModel
    {
        public string FileBase64 { get; set; }
    }

    public class PartnerCategoryImportResponse
    {
        public bool Success { get; set; }
        public IEnumerable<string> Errors { get; set; }
    }
}
