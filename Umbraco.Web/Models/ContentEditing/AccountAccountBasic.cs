using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class AccountAccountBasic
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string CompanyName { get; set; }

        /// <summary>
        /// Mã
        /// </summary>
        public string Code { get; set; }

        public bool Active { get; set; }

        public string Note { get; set; }


    }
}
