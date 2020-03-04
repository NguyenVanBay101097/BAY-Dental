﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class FacebookPageSave
    {
        public long UserId { get; set; }
        public string UserName { get; set; }
        public string UserAccesstoken { get; set; }

        public long PageId { get; set; }

        public string PageName { get; set; }
        public string PageAccesstoken { get; set; }
    }
}
