using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class SaleOrderLineSave
    {
        public SaleOrderLineSave()
        {
        }

        public Guid Id { get; set; }

        public decimal PriceUnit { get; set; }

        public decimal ProductUOMQty { get; set; }

        public string Name { get; set; }

        public decimal Discount { get; set; }

        public Guid? ProductId { get; set; }

        /// <summary>
        /// Chuẩn đoán
        /// </summary>
        public string Diagnostic { get; set; }

        public Guid? ToothCategoryId { get; set; }

        public IEnumerable<Guid> ToothIds { get; set; } = new List<Guid>();


        /// <summary>
        /// danh sách chương trình ưu đãi
        /// </summary>
        public ICollection<SaleOrderPromotionSave> Promotions { get; set; } = new List<SaleOrderPromotionSave>();

        /// <summary>
        /// whole_jaw: nguyên hàm
        /// upper_jaw : hàm trên
        /// lower_jaw : hàm dưới
        /// manually :  thủ công
        /// </summary>
        public string ToothType { get; set; }

        public Guid? EmployeeId { get; set; }

        public Guid? AssistantId { get; set; }

        public Guid? CounselorId { get; set; }

        public bool IsActive { get; set; }
        public Guid OrderId { get; set; }
    }
}
