using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class ServiceCardOrderDefault
    {
        public ServiceCardOrderDefault()
        {
            DateOrder = DateTime.Now;
        }

        public ApplicationUserSimple User { get; set; }
        public DateTime DateOrder { get; set; }
    }
}
