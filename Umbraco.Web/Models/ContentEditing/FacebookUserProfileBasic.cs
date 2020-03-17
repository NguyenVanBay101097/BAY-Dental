using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class FacebookUserProfileBasic
    {
        public string PSId { get; set; }
        public string Name { get; set; }

        public string Gender { get; set; }
        /// <summary>
        /// ngày tạo
        /// </summary>
        public DateTime DateCreated { get; set; }
    }
}
