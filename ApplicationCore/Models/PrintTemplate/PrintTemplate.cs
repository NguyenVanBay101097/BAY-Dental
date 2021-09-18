using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Models.PrintTemplate
{
    public class CompanyPrintTemplate
    {
        public string Name { get; set; }
        public string PartnerName { get; set; }
        public string PartnerCityName { get; set; }
        public string PartnerDistrictName { get; set; }
        public string PartnerWardName { get; set; }
        public string PartnerStreet { get; set; }
        public string Address
        {
            get
            {
                var list = new List<string>();
                if (!string.IsNullOrEmpty(PartnerStreet))
                    list.Add(PartnerStreet);
                if (!string.IsNullOrEmpty(PartnerWardName))
                    list.Add(PartnerWardName);
                if (!string.IsNullOrEmpty(PartnerDistrictName))
                    list.Add(PartnerDistrictName);
                if (!string.IsNullOrEmpty(PartnerCityName))
                    list.Add(PartnerCityName);
                return string.Join(", ", list);
            }
            set { }
        }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Logo { get; set; }
    }

    public class ApplicationUserPrintTemplate
    {
        public string Id { get; set; }

        public string Name { get; set; }
    }

    public class PartnerPrintTemplate
    {
        public string DisplayName { get; set; }
        public string Name { get; set; }
        public string Gender { get; set; }
        public string DateOfBirth { get; set; }
        public string Phone { get; set; }
        public int? BirthYear { get; set; }

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


        public string DisplayGender
        {
            get
            {
                if (Gender == "male")
                    return "Nam";
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

    public class EmployeePrintTemplate
    {
        public Guid Id { get; set; }

        public string Name { get; set; }
    }

    public class ProductSimplePrintTemplate
    {
        public string Name { get; set; }
        public string NameNoSign { get; set; }
        public string DefaultCode { get; set; }
        public decimal? PriceUnit { get; set; }
        public decimal? PurchasePrice { get; set; }
        public double StandardPrice { get; set; }

        public Guid? CategId { get; set; }
        public ProductCategoryBasicPrintTemplate Categ { get; set; }

        public string Type2 { get; set; }


        public decimal? ListPrice { get; set; }

        /// <summary>
        /// hãng:  nếu là labo thì hãng nào ?
        /// </summary>
        public string Firm { get; set; }

        public decimal? LaboPrice { get; set; }

        public Guid UOMId { get; set; }
        public UoMBasicPrintTemplate UOM { get; set; }
    }

    public class ProductCategoryBasicPrintTemplate
    {
        public string Name { get; set; }

        public string CompleteName { get; set; }
    }

    public class UoMBasicPrintTemplate
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public bool Active { get; set; }

        public string UOMType { get; set; }

        public string CategoryName { get; set; }

    }

}
