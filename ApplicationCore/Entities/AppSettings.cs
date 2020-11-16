using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    public class AppSettings
    {
        public string Secret { get; set; }

        public string Domain { get; set; }

        public string Schema { get; set; }

        public string Version { get; set; }
    }
}
