using ApplicationCore.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class LoaiThuChiDisplay
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Code { get; set; }

        public string Note { get; set; }

        public Guid? AccountId { get; set; }
        public AccountAccount Account { get; set; }

        /// <summary>
        /// Hiện trong báo cáo két quả kinh doanh
        /// </summary>
        public bool IsInclude { get; set; }
    }
}
