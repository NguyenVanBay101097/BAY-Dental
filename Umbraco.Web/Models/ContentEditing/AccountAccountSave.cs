using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class AccountAccountSave
    {
        public AccountAccountSave()
        {
            Reconcile = false;
        }

        /// <summary>
        /// Tên
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Mã
        /// </summary>
        public string Code { get; set; }

        public bool Active { get; set; }

        public string Note { get; set; }

        public Guid? UserTypeId { get; set; }

        public Guid? CompanyId { get; set; }
        public CompanySimple Company { get; set; }

        //UserType.Type
        public string InternalType { get; set; }

        public bool Reconcile { get; set; }
    }
}
