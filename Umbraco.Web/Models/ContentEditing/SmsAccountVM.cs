using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class SmsAccountSave
    {

        public string Name { get; set; }
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
        /// Vietguys
        /// </summary>
        public string UserName { get; set; }

        public string Password { get; set; }

        /// <summary>
        /// Config of E-SMS
        /// </summary>
        public string ApiKey { get; set; }
        public string Secretkey { get; set; }
    }

    public class SmsAccountPaged
    {
        public int Limit { get; set; }
        public int Offset { get; set; }
        public string Search { get; set; }
    }

    public class SmsAccountBasic
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string BrandName { get; set; }
    }

    public class SmsAccountDisplay
    {
        public Guid Id { get; set; }
        public Guid? CompanyId { get; set; }
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
        /// Vietguys
        /// </summary>
        public string UserName { get; set; }

        public string Password { get; set; }

        /// <summary>
        /// Config of E-SMS
        /// </summary>
        public string ApiKey { get; set; }
        public string Secretkey { get; set; }
    }
    
    public class SmsSupplierBasic
    {
        public string Name { get; set; }
        public string BrandName { get; set; }
        public string Provider { get; set; }
    }
}
