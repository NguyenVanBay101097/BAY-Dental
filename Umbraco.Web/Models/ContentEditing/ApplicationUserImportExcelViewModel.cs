using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class ApplicationUserImportExcelViewModel
    {
        public string FileBase64 { get; set; }
    }

    public class ApplicationUserImportResponse
    {
        public bool Success { get; set; }
        public IEnumerable<string> Errors { get; set; }
    }
}
