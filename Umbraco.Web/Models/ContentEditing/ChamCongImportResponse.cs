using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class ChamCongImportResponse
    {
        public bool Success { get; set; }
        public IEnumerable<string> Errors { get; set; }
        public ImportFileExcellChamCongModel ModelError { get; set; }
        public string Message { get; set; }
    }

}
