using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class PrintPaperSizeBasic
    {
        public Guid Id { get; set; }
        public string Name { get; set; }

    }

    public class PrintPaperSizePaged
    {
        public PrintPaperSizePaged()
        {
            Limit = 20;
        }

        public int Offset { get; set; }

        public int Limit { get; set; }

        public string Search { get; set; }

    }

    public class PrintPaperSizeSave
    {
        public string Name { get; set; }

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
    }

    public class PrintPaperSizeDisplay
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

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

    }
}
