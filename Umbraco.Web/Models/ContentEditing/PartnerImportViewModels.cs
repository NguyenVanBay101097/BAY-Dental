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

    public class PartnerSupplierRowExcel
    {
        public string Name { get; set; }
        public string Ref { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string Email { get; set; }
        public string Note { get; set; }
        public string Fax { get; set; }
    }

    public class PartnerImportExcelViewModel
    {
        public string FileBase64 { get; set; }
        public string Type { get; set; }
    }

    public class PartnerImportResponse
    {
        public bool Success { get; set; }
        public IEnumerable<string> Errors { get; set; }
    }
}
