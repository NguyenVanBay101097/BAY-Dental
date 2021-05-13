using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    public class SmsAccount : BaseEntity
    {
        public string Name { get; set; }
        public Guid? CompanyId { get; set; }
        public Company Company { get; set; }

        /// <summary>
        /// fpt, esms, vietguys
        /// </summary>
        public string Provider { get; set; }

        public string BrandName { get; set; }

        /// <summary>
        /// Fpt
        /// </summary>
        public string ClientId { get; set; }

        public string ClientSecret { get; set; }

        /// <summary>
        /// Config of E-SMS
        /// </summary>
        public string ApiKey { get; set; }
        public string Secretkey { get; set; }
    }
}
