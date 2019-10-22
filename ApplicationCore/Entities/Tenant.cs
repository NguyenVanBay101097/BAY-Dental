using ApplicationCore.Entities;
using MyERP.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    public class AppTenant
    {
        public AppTenant()
        {
            Id = GuidComb.GenerateComb();
            DateCreated = DateTime.Now;
        }
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }

        public string Phone { get; set; }

        public string CompanyName { get; set; }

        public string Hostname { get; set; }

        public DateTime? DateCreated { get; set; }

        /// <summary>
        /// Ngay het han
        /// </summary>
        public DateTime? DateExpired { get; set; }
    }
}
