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

        /// <summary>
        /// Hiện trong báo cáo két quả kinh doanh
        /// </summary>
        public bool IsInclude { get; set; }

        public Guid? AccountId { get; set; }
        public AccountAccountSave Account { get; set; }

        /// <summary>
        /// Chi nhánh
        /// </summary>
        public Guid? CompanyId { get; set; }
        public CompanySimple Company { get; set; }
    }
}
