using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    public class FacebookConnectPage: BaseEntity
    {
        public string PageId { get; set; }
        public string PageName { get; set; }
        public string PageAccessToken { get; set; }

        public string Picture { get; set; }

        public Guid ConnectId { get; set; }
        public FacebookConnect Connect { get; set; }
    }
}
