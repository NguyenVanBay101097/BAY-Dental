using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class FacebookMassMessagingBasic
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string State { get; set; }
        public int TotalSent { get; set; }
    }
}
