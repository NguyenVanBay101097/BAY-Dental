using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class ToaThuocLineDisplay
    {
        public ToaThuocLineDisplay()
        {
            UseAt = "after_meal";
        }

        public string Name { get; set; }

        public Guid Id { get; set; }

        public Guid ProductId { get; set; }
        public ProductBasic Product { get; set; }

        public Guid? ProductUoMId { get; set; }
        public UoMBasic ProductUoM { get; set; }

        public int? Sequence { get; set; }

        /// <summary>
        /// Số lần uống 1 ngày
        /// </summary>
        public int NumberOfTimes { get; set; }

        /// <summary>
        /// Số ngày
        /// </summary>
        public int NumberOfDays { get; set; }

        /// <summary>
        /// Số lượng uống 1 lần
        /// </summary>
        public decimal AmountOfTimes { get; set; }

        /// <summary>
        /// Số lượng
        /// </summary>
        public decimal Quantity { get; set; }

        /// <summary>
        /// Dùng lúc
        /// Sau khi ăn: after_meal
        /// Trước khi ăn: before_meal
        /// Trong khi ăn: in_meal
        /// Sau khi dậy: after_wakeup
        /// Trước khi đi ngủ: before_sleep
        /// </summary>
        public string UseAt { get; set; }

        public string UseAtDisplay()
        {
            switch (this.UseAt)
            {
                case "after_meal": return "Sau khi ăn";
                case "before_meal": return "Trước khi ăn";
                case "in_meal": return "Trong khi ăn";
                case "after_wakeup": return "Sau khi dậy";
                case "before_sleep": return "Trước khi đi ngủ";
                default:
                    return "";
            }
        }
        /// <summary>
        /// Ghi chú
        /// </summary>
        public string Note { get; set; }
    }

    public class ToaThuocLineOnChangeProductRequest
    {
        public Guid? ProductId { get; set; }
    }

    public class ToaThuocLineOnChangeProductResponse
    {
        public UoMBasic UoM { get; set; }
    }
}
