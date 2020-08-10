using ApplicationCore.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class EmployeeSimple
    {
        public Guid Id { get; set; }

        public string Name { get; set; }
    }

    public class EmployeeSimpleContact
    {
        public Guid Id { get; set; }

        public string Name { get; set; }
        public string Phone { get; set; }
    }


    public class EmployeeBasic
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        /// <summary>
        /// Mã
        /// </summary>
        public string Ref { get; set; }

        public string Phone { get; set; }


        public string Email { get; set; }
        public Guid? CategoryId { get; set; }
        public EmployeeCategoryBasic Category { get; set; }
    }

    public class EmployeeDisplay
    {
        public EmployeeDisplay()
        {
            IsDoctor = false;
            IsAssistant = false;
        }
        public Guid Id { get; set; }

        public string Name { get; set; }

        /// <summary>
        /// Mã
        /// </summary>
        public string Ref { get; set; }

        /// <summary>
        /// Địa chỉ
        /// </summary>
        public string Address { get; set; }

        public string Phone { get; set; }

        /// <summary>
        /// CMND
        /// </summary>
        public string IdentityCard { get; set; }

        public string Email { get; set; }

        public DateTime? BirthDay { get; set; }

        public Guid? CategoryId { get; set; }
        public EmployeeCategoryBasic Category { get; set; }

        /// <summary>
        /// Là bác sĩ
        /// </summary>
        public bool? IsDoctor { get; set; }

        /// <summary>
        /// Là phụ tá
        /// </summary>
        public bool? IsAssistant { get; set; }
        public IEnumerable<ChamCongDisplay> ChamCongs { get; set; }
        public double SoNgayCong { get; set; }

    }

    public class EmployeePaged
    {
        public EmployeePaged()
        {
            Limit = 20;
        }
        public int Offset { get; set; }
        public int Limit { get; set; }
        public string Search { get; set; }

        //group filter IsDoctor và IsAssistant
        public bool? IsDoctor { get; set; }
        public bool? IsAssistant { get; set; }
        //public bool? IsOther { get; set; }
        public string Position { get; set; }
    }
}
