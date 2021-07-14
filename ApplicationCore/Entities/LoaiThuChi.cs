using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    public class LoaiThuChi: BaseEntity
    {
        public LoaiThuChi()
        {
            IsInclude = true;
        }

        public string Name { get; set; }

        public string Code { get; set; }

        public string Note { get; set; }

        /// <summary>
        /// thu: Loại thu
        /// chi: Loại chi
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Hiện trong báo cáo két quả kinh doanh
        /// </summary>
        public bool IsInclude { get; set; }

        public Guid? AccountId { get; set; }
        public AccountAccount Account { get; set; }

        /// <summary>
        /// hạch toán
        /// </summary>
        public bool IsAccounting { get; set; }

        /// <summary>
        /// Chi nhánh
        /// </summary>
        public Guid? CompanyId { get; set; }
        public Company Company { get; set; }
    }
}
