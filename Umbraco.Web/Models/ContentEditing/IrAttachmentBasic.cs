using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class IrAttachmentBasic
    {
        public Guid Id { get; set; }

        /// <summary>
        /// Attachment Name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// File Name
        /// </summary>
        public string DatasFname { get; set; }

        public string MineType { get; set; }

        public string Url { get; set; }
        public string UploadId { get; set; }

        public DateTime? DateCreated { get; set; }
    }
}
