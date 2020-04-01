using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class FacebookMassMessagingStatisticsPaged
    {
        public FacebookMassMessagingStatisticsPaged()
        {
            Limit = 20;
        }
        public int Offset { get; set; }
        public int Limit { get; set; }
    }
}
