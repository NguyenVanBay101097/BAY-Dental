using ApplicationCore.Entities;
using MyERP.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    public class AppTenant: AdminBaseEntity
    {
        public string Name { get; set; }

        public string Email { get; set; }

        public string Phone { get; set; }

        public string CompanyName { get; set; }

        public string Hostname { get; set; }

        /// <summary>
        /// Ngay het han
        /// </summary>
        //public DateTime? DateExpired { get; set; }
    }
}
