using ApplicationCore.Entities;
using MyERP.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    public class AppTenant: AdminBaseEntity
    {
        public AppTenant()
        {
            Version = "1.0";
            ActiveCompaniesNbr = 1;
        }

        public string Name { get; set; }

        public string Email { get; set; }

        public string Phone { get; set; }

        public string CompanyName { get; set; }

        public string Hostname { get; set; }

        /// <summary>
        /// Ngay het han
        /// </summary>
        public DateTime? DateExpired { get; set; }

        public string Version { get; set; }

        public int ActiveCompaniesNbr { get; set; }

        /// <summary>
        /// Nguồn khách hàng
        /// </summary>
        public string CustomerSource { get; set; }

        /// <summary>
        /// Người triển khai
        /// </summary>
        public Guid? EmployeeId { get; set; }
        public EmployeeAdmin EmployeeAdmin { get; set; }

        public string Address { get; set; }
    }
}
