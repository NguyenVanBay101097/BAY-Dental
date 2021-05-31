﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class PhieuThuChiBasic
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public DateTime Date { get; set; }

        public string PayerReceiver { get; set; }

        /// <summary>
        /// loại thu chi
        /// </summary>
        public string LoaiThuChiName { get; set; }
        /// <summary>
        /// tên phương thức 
        /// </summary>
        public string JournalName { get; set; }

        /// <summary>
        /// tên khach hang
        /// </summary>
        public string CustomerName { get; set; }

        /// <summary>
        /// Số tiền
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// draft: Nháp
        /// posted: Đã vào sổ
        /// </summary>
        public string State { get; set; }
    }
}
