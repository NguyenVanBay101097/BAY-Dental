using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    public class ConfigPrint : BaseEntity
    {

        /// <summary>
        /// khổ giấy in
        /// </summary>
        public Guid? PaperSizeId { get; set; }
        public PrintPaperSize PrintPaperSize { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// ẩn thông tin công ty
        /// </summary>
        public bool IsInfoCompany { get; set; }

        public Guid CompanyId { get; set; }
        public Company Company { get; set; }

    }
}
