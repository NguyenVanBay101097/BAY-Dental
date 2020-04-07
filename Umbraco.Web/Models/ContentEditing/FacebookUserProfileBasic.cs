using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class FacebookUserProfileBasic
    {
        public Guid Id { get; set; }
        public string PSId { get; set; }
        public string Name { get; set; }

        public string Gender { get; set; }
        /// <summary>
        /// ngày tạo
        /// </summary>
        public DateTime DateCreated { get; set; }
        public Guid? PartnerId { get; set; }
    }
}
