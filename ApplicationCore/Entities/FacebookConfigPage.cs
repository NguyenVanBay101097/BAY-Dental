using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    public class FacebookConfigPage: BaseEntity
    {
        public Guid ConfigId { get; set; }
        public FacebookConfig Config { get; set; }

        public string PageName { get; set; }

        public string PageId { get; set; }

        public string PageAccessToken { get; set; }
    }
}
