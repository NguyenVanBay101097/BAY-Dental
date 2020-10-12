using ApplicationCore.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class PartnerBasic
    {
   

        public Guid Id { get; set; }

        public string Name { get; set; }
       

        public string Phone { get; set; }

        /// <summary>
        /// Mã khách hàng
        /// </summary>
        public string Ref { get; set; }

        public string Gender { get; set; }

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

        public string Street { get; set; }

        public string WardName { get; set; }

        public string DistrictName { get; set; }

        public string CityName { get; set; }

        public int? BirthYear { get; set; }

        /// <summary>
        /// Công nợ
        /// </summary>
        public decimal Debt { get; set; }

        public string DisplayName { get; set; }

        public string Age
        {
            get
            {
                if (!BirthYear.HasValue)
                {
                    return string.Empty;
                }

                return (DateTime.Now.Year - BirthYear.Value).ToString();
            }
            set
            {
            }
        }

        public DateTime? LastAppointmentDate { get; set; }

        public decimal Debit { get; set; }

        public decimal Credit { get; set; }
    }

    public class PartnerPaged
    {
        public PartnerPaged()
        {
            Limit = 20;
        }

        public int Offset { get; set; }

        public int Limit { get; set; }

        public string Search { get; set; }

        public bool? Customer { get; set; }

        public bool? Employee { get; set; }

        public bool? Supplier { get; set; }

        public Guid? CategoryId { get; set; }

        public bool ComputeCreditDebit { get; set; }
    }

    public class PartnerPatch
    {
        public string Avatar { get; set; }
    }

    public class PartnerImportExcel
    {
        public PartnerImportExcel()
        {
            Active = true;
            Employee = false;
        }
        public string Name { get; set; }

        public string NameNoSign { get; set; }

        public string Phone { get; set; }

        public string Fax { get; set; }

        public string Ref { get; set; }

        public string Gender { get; set; }

        public string Street { get; set; }

        public string CityName { get; set; }
        public string CityCode { get; set; }

        public string DistrictCode { get; set; }
        public string DistrictName { get; set; }

        public string WardCode { get; set; }
        public string WardName { get; set; }

        public string BirthDay { get; set; }

        public string BirthMonth { get; set; }

        public string BirthYear { get; set; }

        public string MedicalHistory { get; set; }

        public string JobTitle { get; set; }

        public string Email { get; set; }

        public string Comment { get; set; }

        public bool Supplier { get; set; }

        public bool Customer { get; set; }

        public bool Active { get; set; }

        public bool Employee { get; set; }
    }

    public class PartnerAddRemoveTagsVM
    {
        public Guid Id { get; set; }
        public IEnumerable<Guid> TagIds { get; set; } = new List<Guid>();
    }

    public class PartnerPrintVM
    {
        public string DisplayName { get; set; }
        public string Name { get; set; }
        public string Gender { get; set; }
        public string DateOfBirth { get; set; }
        public string Phone { get; set; }
        public int? BirthYear { get; set; }

        public int? Age
        {
            get
            {
                return BirthYear.HasValue ? DateTime.Now.Year - BirthYear.Value : (int?)null;
            }
            set { }
        }

        public string DisplayGender
        {
            get
            {
                if (Gender == "male")
                    return "Name";
                else if (Gender == "female")
                    return "Nữ";
                else
                    return "Khác";
            }
            set { }
        }

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

        public string Street { get; set; }

        public string WardName { get; set; }

        public string DistrictName { get; set; }

        public string CityName { get; set; }
    }
}
