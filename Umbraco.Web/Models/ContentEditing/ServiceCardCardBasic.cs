using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class ServiceCardCardBasic
    {
        public Guid Id { get; set; }

        /// <summary>
        /// Số thẻ
        /// </summary>
        public string Name { get; set; }
        public string State { get; set; }
        public Guid CardTypeId { get; set; }
        public ServiceCardTypeSimple CardType { get; set; }
        public Guid? PartnerId { get; set; }
        public PartnerSimple Partner { get; set; }
    }
}
