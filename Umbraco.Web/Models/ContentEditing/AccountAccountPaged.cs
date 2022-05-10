using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class AccountAccountGetListCanPayOrReceiveRequest
    {
        public AccountAccountGetListCanPayOrReceiveRequest()
        {
            Limit = 20;
        }
        public int Offset { get; set; }
        public int Limit { get; set; }
  
        public string Search { get; set; }

        public Guid? CompanyId { get; set; }
    }

    public class AccountAccountThuChiPaged
    {
        public AccountAccountThuChiPaged()
        {
            Limit = 20;
            Type = "thu";
        }

        public int Offset { get; set; }

        public int Limit { get; set; }

        /// <summary>
        /// thu
        /// chi
        /// </summary>
        public string Type { get; set; }

        public string Search { get; set; }
    }
}
