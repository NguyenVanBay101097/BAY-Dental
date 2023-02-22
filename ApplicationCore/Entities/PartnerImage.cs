using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    public class PartnerImage : BaseEntity
    {
        public PartnerImage()
        {
            Date = DateTime.Now;
        }
        /// <summary>
        /// Tên hình ảnh
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Ghi chú hình ảnh
        /// </summary>
        public string Note { get; set; }

        /// <summary>
        /// Ngày upload Hình
        /// </summary>
        public DateTime? Date { get; set; }

        /// <summary>
        /// Tham chiếu tới 1 partner
        /// </summary>
        public Guid? PartnerId { get; set; }
        public Partner Partner { get; set; }
        /// <summary>
        /// Tham chiếu tới 1 đợt khám
        /// </summary>
        public Guid? DotkhamId { get; set; }
        public DotKham DotKham { get; set; }

        /// <summary>
        /// Id của image khi up len server
        /// </summary>
        public string UploadId { get; set; }
    }
}
