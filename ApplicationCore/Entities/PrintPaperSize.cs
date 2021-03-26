using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    public class PrintPaperSize : BaseEntity
    {
        public string Name { get; set; }

        ///// <summary>
        ///// A5
        ///// A4
        ///// </summary>
        public string PaperFormat { get; set; }

        /// <summary>
        /// canh lề trên
        /// </summary>
        public int TopMargin { get; set; }

        /// <summary>
        /// canh lề dưới
        /// </summary>
        public int BottomMargin { get; set; }

        /// <summary>
        /// canh lề trái
        /// </summary>
        public int LeftMargin { get; set; }

        /// <summary>
        /// canh lề phải
        /// </summary>
        public int RightMargin { get; set; }

        public Guid CompanyId { get; set; }
        public Company Company { get; set; }
    }
}
