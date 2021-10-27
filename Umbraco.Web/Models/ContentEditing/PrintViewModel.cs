using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class PrintData
    {
        public string html { get; set; }
        public IEnumerable<IrAttachmentBasic> IrAttachments { set; get; } = new List<IrAttachmentBasic>();
    }
}
