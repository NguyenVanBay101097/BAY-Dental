using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class ImportFileExcellChamCongModel
    {
        /// <summary>
        /// Mã nhân viên like trong bảng Employee
        /// </summary>
        public string MaNV { get; set; }

        /// <summary>
        /// Id của máy chấm công
        /// </summary>
        public string IdMayChamCong { get; set; }

        /// <summary>
        /// Ngày chấm công
        /// </summary>
        public DateTime? Date { get; set; }


        /// <summary>
        /// Thời gian vào/ra
        /// </summary>
        public DateTime Time { get; set; }

        /// <summary>
        /// Kiều châm công: 
        /// 1: check-in - thời gian vào
        /// 2: check-out - thời giân ra
        /// 3: break-in - bắt đầu thời gian giải lao
        /// 4: break-out - kết thúc thời gian giải lao
        /// 5: ot-in - tăng ca vào
        /// 6: ot-out - tăng ca ra
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// code : tkp
        /// </summary>
        public string CodeWorkEntryType { get; set; }

        public int Stt { get; set; }
    }
}
