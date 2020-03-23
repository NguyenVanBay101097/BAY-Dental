using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class PartnerCustomerRowExcel
    {
        public string Name { get; set; }
        public string Ref { get; set; }
        public string Gender { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string MedicalHistory { get; set; }
        public string Job { get; set; }
        public string Email { get; set; }
        public string Note { get; set; }
    }
}
