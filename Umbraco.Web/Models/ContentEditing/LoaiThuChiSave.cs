using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class LoaiThuChiSave
    {
        public string Name { get; set; }

        public string Code { get; set; }

        public string Note { get; set; }

        public string Type { get; set; }

        /// <summary>
        /// Chi nhánh
        /// </summary>
        public Guid? CompanyId { get; set; }

        public bool IsAccounting { get; set; }
    }
}
