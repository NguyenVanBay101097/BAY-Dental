using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class ResInsurancePaged
    {
        public ResInsurancePaged()
        {
            Limit = 20;
        }

        public int Offset { get; set; }

        public int Limit { get; set; }

        public string Search { get; set; }

        public bool? IsActive { get; set; }

        public bool? IsDebt { get; set; }

    }

    public class ResInsuranceBasic
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Phone { get; set; }

        public decimal? TotalDebt { get; set; }

        public bool IsActive { get; set; }
    }


    public class ResInsuranceDisplay
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public DateTime Date { get; set; }

        public string Avatar { get; set; }

        public Guid PartnerId { get; set; }

        /// <summary>
        /// người đại diện
        /// </summary>
        public string Representative { get; set; }

        public string Phone { get; set; }
        public string Email { get; set; }

        public string Address { get; set; }

        public string Note { get; set; }
    }

    public class ResInsuranceSave
    {
        public string Name { get; set; }

        public DateTime Date { get; set; }

        public string Avatar { get; set; }

        /// <summary>
        /// người đại diện
        /// </summary>
        public string Representative { get; set; }

        public string Phone { get; set; }
        public string Email { get; set; }

        public string Address { get; set; }

        public string Note { get; set; }
    }

    public class InsuranceIsActivePatch
    {
        public bool IsActive { get; set; }
    }

    public class ResInsuranceSimple
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
    }

    public class InsuranceActionDeactiveRequest
    {
        public IEnumerable<Guid> Ids { get; set; }
    }
}
