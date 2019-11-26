using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    /// <summary>
    /// Attachments are used to link binary files or url to any openerp document.
    /// </summary>
    public class IrAttachment: BaseEntity
    {
        public IrAttachment()
        {
            Active = true;
            Type = "binary";
        }

        /// <summary>
        /// Attachment Name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// File Name
        /// </summary>
        public string DatasFname { get; set; }

        public string Description { get; set; }

        public string ResName { get; set; }

        public string ResField { get; set; }

        public string ResModel { get; set; }

        public Guid? ResId { get; set; }

        public Guid? CompanyId { get; set; }
        public Company Company { get; set; }

        public string Type { get; set; }

        public string Url { get; set; }

        public byte[] DbDatas { get; set; }

        public string MineType { get; set; }

        public bool Active { get; set; }

        public int? FileSize { get; set; }

        public string UploadId { get; set; }
    }
}
