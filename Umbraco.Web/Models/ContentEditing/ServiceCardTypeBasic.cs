using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class ServiceCardTypeBasic
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        /// <summary>
        /// Loại
        /// cash: Thẻ tiền mặt
        /// </summary>
        public string Type { get; set; }
    }
}
