using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class DotKhamPublicFilter
    {
        public Guid SaleOrderId { get; set; }
    }

    public class DotKhamPublic
    {
        public Guid Id { get; set; }

        /// <summary>
        /// Số đợt khám
        /// </summary>
        public string Name { get; set; }

        public DateTime Date { get; set; }

        public string DoctorName { get; set; }

        public string AssistantName { get; set; }

        public IEnumerable<DotKhamLinePublic> Lines = new List<DotKhamLinePublic>();

        /// <summary>
        /// Mô tả
        /// </summary>
        public string Reason { get; set; }

        public IEnumerable<string> Images = new List<string>();
    }
}
