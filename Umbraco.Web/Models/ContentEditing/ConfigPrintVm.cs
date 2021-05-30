using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class ConfigPrintBasic
    {
        public Guid Id { get; set; }

        /// <summary>
        /// khổ giấy in
        /// </summary>
        public Guid? PaperSizeId { get; set; }
        public PrintPaperSizeBasic PrintPaperSize { get; set; }

        public string Name { get; set; }

        /// <summary>
        /// ẩn thông tin công ty
        /// </summary>
        public bool IsInfoCompany { get; set; }

        public string Code { get; set; }


    }

    public class ConfigPrintDisplay
    {
        public Guid Id { get; set; }

        /// <summary>
        /// khổ giấy in
        /// </summary>
        public Guid? PaperSizeId { get; set; }
        public PrintPaperSizeDisplay PrintPaperSize { get; set; }

        public string Name { get; set; }

        /// <summary>
        /// ẩn thông tin công ty
        /// </summary>
        public bool IsInfoCompany { get; set; }


    }

    public class ConfigPrintSave
    {
        public Guid Id { get; set; }

        /// <summary>
        /// khổ giấy in
        /// </summary>
        public Guid? PaperSizeId { get; set; }

        public string Name { get; set; }

        /// <summary>
        /// ẩn thông tin công ty
        /// </summary>
        public bool IsInfoCompany { get; set; }

        public Guid CompanyId { get; set; }
    }
}
