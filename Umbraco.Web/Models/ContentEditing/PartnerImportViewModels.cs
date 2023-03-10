using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class PartnerImportRowExcel
    {
        public string Name { get; set; }
        public string Ref { get; set; }
        public DateTime? Date { get; set; }
        public string Gender { get; set; }
        public int? BirthYear { get; set; }
        public int? BirthMonth { get; set; }
        public int? BirthDay { get; set; }
        public string Phone { get; set; }
        public string Street { get; set; }
        public string CityName { get; set; }
        public string DistrictName { get; set; }
        public string WardName { get; set; }
           
        public string Address
        {
            get
            {
                var list = new List<string>();
                if (!string.IsNullOrEmpty(WardName))
                    list.Add(WardName);
                if (!string.IsNullOrEmpty(DistrictName))
                    list.Add(DistrictName);
                if (!string.IsNullOrEmpty(CityName))
                    list.Add(CityName);
                return string.Join(", ", list);
            }
            set { }
        }
      
        public string MedicalHistory { get; set; }
        public string Job { get; set; }
        public string Email { get; set; }
        public string Note { get; set; }
        public string Fax { get; set; }
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

        public bool CheckAddress { get; set; }
    }

    public class PartnerImportResponse
    {
        public bool Success { get; set; }
        public IEnumerable<string> Errors { get; set; }
    }

    public class PartnerCustomerExportExcelVM
    {
        public Guid Id { get; set; }
        public string Name { get; set; }

        public string Ref { get; set; }

        public string Gender { get; set; }
      
        public string Phone { get; set; }

        public string Job { get; set; }

        public string Email { get; set; }

        public string Note { get; set; }

        public IEnumerable<string> MedicalHistories { get; set; } = new List<string>();

        public int? BirthYear { get; set; }

        public int? BirthMonth { get; set; }

        public int? BirthDay { get; set; }

        public string DateOfBirth 
        {
            get
            {
                var list = new List<string>();
                if (BirthDay.HasValue)
                    list.Add(BirthDay.Value.ToString());
                if (BirthMonth.HasValue)
                    list.Add(BirthMonth.Value.ToString());
                if (BirthYear.HasValue)
                    list.Add(BirthYear.Value.ToString());
                return string.Join("/", list);
            }
        }

        public string Street { get; set; }

        public string WardName { get; set; }

        public string DistrictName { get; set; }

        public string CityName { get; set; }

        public string Address
        {
            get
            {
                var list = new List<string>();
                if (!string.IsNullOrEmpty(Street))
                    list.Add(Street);
                if (!string.IsNullOrEmpty(WardName))
                    list.Add(WardName);
                if (!string.IsNullOrEmpty(DistrictName))
                    list.Add(DistrictName);
                if (!string.IsNullOrEmpty(CityName))
                    list.Add(CityName);
                return string.Join(", ", list);
            }
            set { }
        }

        public string CheckAddress
        {
            get
            {
                if (string.IsNullOrWhiteSpace(WardName) || string.IsNullOrWhiteSpace(DistrictName) || string.IsNullOrWhiteSpace(CityName))
                    return string.Empty;
                var list = new List<string>();
                list.Add(WardName);
                list.Add(DistrictName);
                list.Add(CityName);
                return string.Join(", ", list);
            }
            set { }
        }

        public DateTime? Date { get; set; }
        public string TitleName { get; set; }
        public string SourceName { get; set; }

        public IEnumerable<PartnerCategoryBasic> Categories { get; set; } = new List<PartnerCategoryBasic>();
        public string Tags
        {
            get
            {
                var list = new List<string>();
                foreach (var item in this.Categories)
                {
                    list.Add(item.Name);
                }
                return string.Join(", ", list);
            }
            set { }
        }

        public string MedicalHistory { get; set; }

        public string PartnerCategories { get; set; }
    }

    public class PartnerCustomerImportRowExcel
    {
        public string Name { get; set; }
        public string Ref { get; set; }
        public DateTime? Date { get; set; }
        public string Gender { get; set; }
        public int? BirthYear { get; set; }
        public int? BirthMonth { get; set; }
        public int? BirthDay { get; set; }
        public string Phone { get; set; }
        public string Street { get; set; }
        public string CityName { get; set; }
        public string DistrictName { get; set; }
        public string WardName { get; set; }

        public string CheckAddress
        {
            get
            {
                var list = new List<string>();
                if (!string.IsNullOrEmpty(WardName))
                    list.Add(WardName);
                if (!string.IsNullOrEmpty(DistrictName))
                    list.Add(DistrictName);
                if (!string.IsNullOrEmpty(CityName))
                    list.Add(CityName);
                return string.Join(", ", list);
            }
            set { }
        }

        public string MedicalHistory { get; set; }
        public string Job { get; set; }
        public string Email { get; set; }
        public string Note { get; set; }
    }

    public class PartnerSupplierImportRowExcel
    {
        public string Name { get; set; }
        public DateTime? Date { get; set; }
        public string Phone { get; set; }
        public string Street { get; set; }
        public string CityName { get; set; }
        public string DistrictName { get; set; }
        public string WardName { get; set; }
        public string Email { get; set; }
        public string Note { get; set; }
        public string Fax { get; set; }
        public string Ref { get; set; }
    }

    public class WardVm
    {
        public string CityCode { get; set; }
        public string CityName { get; set; }
        public string DistrictCode { get; set; }
        public string DistrictName { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string NameNoSign { get; set; }
    }

    public class DistrictVm
    {
        public string CityCode { get; set; }
        public string CityName { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string NameNoSign { get; set; }

    }

    public class CityVm
    {

        public string Code { get; set; }
        public string Name { get; set; }
        public string NameNoSign { get;set; }

    }
}
