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

        public string DisplayName { get; set; }

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

        public decimal? Balance { get; set; }
        public string CompanyName { get; set; }
        public IEnumerable<PartnerCategoryBasic> Categories { get; set; } = new List<PartnerCategoryBasic>();
        public DateTime? DateCreated { get; set; }
        public PartnerSourceBasic Source { get; set; }
        public string JobTitle { get; set; }
        public string Email { get; set; }
        public string comment { get; set; }
        public int? BirthMonth { get; set; }
        public int? BirthDay { get; set; }

        public bool Active { get; set; }

        public decimal CountLine { get; set; }
        public string DateOfBirth { get; set; }

        public Guid AppointmnetId { get; set; }
        public string AppointmnetName { get; set; }
        public string Time { get; set; }
        public string Date { get; set; }

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

        public IEnumerable<Guid> TagIds { get; set; } = new List<Guid>();

        public bool ComputeCreditDebit { get; set; }
        public bool? Active { get; set; }
        public Guid? CompanyId { get; set; }
    }

    public class PartnerGetDebtPagedFilter
    {
        public int Limit { get; set; }

        public int Offset { get; set; }

        public string Search { get; set; }

        public Guid? CompanyId { get; set; }
    }

    public class PartnerGetDebtPagedItem
    {
        public string PartnerName { get; set; }
        public DateTime? Date { get; set; }

        public decimal AmountResidual { get; set; }

        public decimal Balance { get; set; }

        public string Origin { get; set; }

        public string Ref { get; set; }

        public Guid MoveId { get; set; }
        public string MoveType { get; set; }
    }


    public class PartnerSaleOrderDone
    {
        public Guid Id { get; set; }
        public string DisplayName { get; set; }
        public string Phone { get; set; }
        public string SaleOrderName { get; set; }
        public Guid? SaleOrderId { get; set; }
        public DateTime? DateDone { get; set; }
        public string SaleOrderLineName { get; set; }
        public IEnumerable<SaleOrderLineSmsSimple> SaleOrderLines { get; set; } = new List<SaleOrderLineSmsSimple>();
        public decimal? Debit { get; set; }
    }

    public class PartnerPatch
    {
        public string Avatar { get; set; }
    }

    public class PartnerActivePatch
    {
        public bool Active { get; set; }
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
        public int? BirthMonth { get; set; }
        public int? BirthDay { get; set; }
        public string DateOfBirth
        {
            get
            {
                if (!BirthDay.HasValue && !BirthMonth.HasValue && !BirthYear.HasValue) return "";
                return $"{(BirthDay.HasValue ? BirthDay.Value.ToString() : "--")}/" +
                    $"{(BirthMonth.HasValue ? BirthMonth.Value.ToString() : "--")}/" +
                    $"{(BirthYear.HasValue ? BirthYear.Value.ToString() : "----")}";
            }
            set
            {

            }
        }

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
        public string Email { get; set; }
        public string Ref { get; set; }

    }

    public class PartnerViewModel
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string DisplayName { get; set; }

        public string NameNoSign { get; set; }

        public string Ref { get; set; }

        public string Phone { get; set; }

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

        public bool Customer { get; set; }

        public bool Supplier { get; set; }

        public string Street { get; set; }

        public string WardName { get; set; }

        public string DistrictName { get; set; }

        public string CityName { get; set; }

        public int? BirthMonth { get; set; }

        public int? BirthDay { get; set; }

        public int? BirthYear { get; set; }

        public string DateOfBirth
        {
            get
            {
                return $"{(BirthDay.HasValue ? BirthDay.Value.ToString() : "_")}/" +
                    $"{(BirthMonth.HasValue ? BirthMonth.Value.ToString() : "_")}/" +
                    $"{(BirthYear.HasValue ? BirthYear.Value.ToString() : "_")}";
            }
            set { }
        }

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

        public string Gender { get; set; }

        public string Comment { get; set; }

        public string Email { get; set; }

        public string JobTitle { get; set; }
        public DateTime? LastAppointmentDate { get; set; }

        public string GenderDisplay
        {
            get
            {
                switch (Gender)
                {
                    case "female":
                        return "Nữ";
                    case "other":
                        return "Khác";
                    default:
                        return "Nam";
                }
            }
            set
            {
            }
        }

        public DateTime? Date { get; set; }
        public DateTime? DateCreated { get; set; }
        //public PartnerSourceViewModel Source { get; set; }

        public IEnumerable<PartnerCategoryViewModel> Tags { get; set; } = new List<PartnerCategoryViewModel>();
    }

    public class GridPartnerViewModel
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string DisplayName { get; set; }

        public string NameNoSign { get; set; }

        public string Ref { get; set; }

        public string Phone { get; set; }

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

        public bool Customer { get; set; }

        public bool Supplier { get; set; }

        public string Street { get; set; }

        public string WardName { get; set; }

        public string DistrictName { get; set; }

        public string CityName { get; set; }

        public int? BirthMonth { get; set; }

        public int? BirthDay { get; set; }

        public int? BirthYear { get; set; }

        public string DateOfBirth
        {
            get
            {
                if (!BirthDay.HasValue && !BirthMonth.HasValue && !BirthYear.HasValue) return "";
                return $"{(BirthDay.HasValue ? BirthDay.Value.ToString() : "--")}/" +
                    $"{(BirthMonth.HasValue ? BirthMonth.Value.ToString() : "--")}/" +
                    $"{(BirthYear.HasValue ? BirthYear.Value.ToString() : "----")}";
            }
            set { }
        }

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

        public string Gender { get; set; }

        public string Comment { get; set; }

        public string Email { get; set; }

        public string JobTitle { get; set; }

        public DateTime? LastAppointmentDate { get; set; }

        public string GenderDisplay
        {
            get
            {
                switch (Gender)
                {
                    case "female":
                        return "Nữ";
                    case "other":
                        return "Khác";
                    default:
                        return "Nam";
                }
            }
            set
            {
            }
        }

        public DateTime? Date { get; set; }
        public DateTime? DateCreated { get; set; }

        public string SourceName { get; set; }

        public string Tags { get; set; }
    }

    public class PartnerCustomerReportInput
    {
        public DateTime? DateFrom { get; set; }
        public Guid? CompanyId { get; set; }
        public DateTime? DateTo { get; set; }
    }

    public class PartnerQueryableFilter
    {
        public PartnerQueryableFilter()
        {
            Limit = 20;
        }

        public int Limit { get; set; }
        public int Offset { get; set; }

        public string Search { get; set; }

        /// <summary>
        /// chi nhanh
        /// </summary>
        public Guid? CompanyId { get; set; }

        public DateTime? DateFrom { get; set; }

        public DateTime? DateTo { get; set; }

        /// <summary>
        /// tuổi 
        /// </summary>
        public decimal? AgeFrom { get; set; }

        public decimal? AgeTo { get; set; }

        public decimal? RevenueFrom { get; set; }
        public decimal? RevenueTo { get; set; }

        /// <summary>
        /// loc theo tong tien dieu tri
        /// </summary>
        public decimal? AmountTotalFrom { get; set; }
        public decimal? AmountTotalTo { get; set; }

        /// <summary>
        /// cong no
        /// </summary>
        public bool? IsDebt { get; set; }

        /// <summary>
        /// Xác định dự kiến thu lớn hơn 0 hoặc bằng 0
        /// </summary>
        public bool? IsRevenueExpect { get; set; }

        /// <summary>
        /// sale : đang điều trị
        /// done : hoàn thành
        /// draft : chưa phát sinh
        /// </summary>
        public string OrderState { get; set; }

        /// <summary>
        /// gioi tinh
        /// </summary>
        public string Gender { get; set; }

        public IEnumerable<Guid> CategIds { get; set; } = new List<Guid>();

        public IEnumerable<Guid> PartnerSourceIds { get; set; } = new List<Guid>();

        public IEnumerable<Guid> CardTypeIds { get; set; } = new List<Guid>();

        /// <summary>
        /// theo doi
        /// </summary>
        public bool? IsActive { get; set; }


        /// <summary>
        /// mã thành phố
        /// </summary>
        public string CityCode { get; set; }

        /// <summary>
        /// mã quận/ huyện
        /// </summary>
        public string DistrictCode { get; set; }

        /// <summary>
        /// mã phường/ xã
        /// </summary>
        public string WardCode { get; set; }

        public bool? CityCodeIsNull { get; set; }

        public bool? DistrictCodeIsNull { get; set; }

        public bool? WardCodeIsNull { get; set; }
    }

    public class PartnerCustomerReportOutput
    {
        public int CustomerOld { get; set; }
        public int CustomerNew { get; set; }
    }

    public class CustomerStatisticsInput
    {
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
    }

    public class CustomerStatisticsDetails
    {
        public string Location { get; set; }
        public int CustomerTotal { get; set; }
        public int CustomerOld { get; set; }
        public int CustomerNew { get; set; }
    }

    public class CustomerStatisticsOutput
    {
        public int CustomerTotal { get; set; }
        public int CustomerOld { get; set; }
        public int CustomerNew { get; set; }
        public IEnumerable<CustomerStatisticsDetails> Details { get; set; } = new List<CustomerStatisticsDetails>();
    }



    public class PartnerInfoVm
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string DisplayName { get; set; }

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

        public string Email { get; set; }

        //public PartnerSourceViewModel Source { get; set; }

        public IEnumerable<PartnerCategoryViewModel> Tags { get; set; } = new List<PartnerCategoryViewModel>();
    }

    public class PublicPartnerRequest
    {
        public PublicPartnerRequest()
        {
            Limit = 20;
        }

        public int Offset { get; set; }

        public int Limit { get; set; }

        public string Search { get; set; }
    }

    public class PublicPartnerReponse
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Avatar { get; set; }

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
        }

        public string Street { get; set; }

        public string WardName { get; set; }

        public string DistrictName { get; set; }

        public string CityName { get; set; }

        public int? BirthYear { get; set; }
        public int? BirthMonth { get; set; }
        public int? BirthDay { get; set; }

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
        }
        public DateTime? Date { get; set; }
        public string JobTitle { get; set; }
        public string Email { get; set; }
        public string DateOfBirth
        {
            get
            {
                return $"{(BirthDay.HasValue ? BirthDay.Value.ToString() : "_")}/" +
                    $"{(BirthMonth.HasValue ? BirthMonth.Value.ToString() : "_")}/" +
                    $"{(BirthYear.HasValue ? BirthYear.Value.ToString() : "_")}";
            }
        }

    }

    public class PublicPartnerInfo
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Avatar { get; set; }

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
        }

        public string Street { get; set; }

        public string WardName { get; set; }

        public string DistrictName { get; set; }

        public string CityName { get; set; }

        public int? BirthYear { get; set; }
        public int? BirthMonth { get; set; }
        public int? BirthDay { get; set; }

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
        }
        public DateTime? Date { get; set; }
        public string JobTitle { get; set; }
        public string Email { get; set; }
        public string DateOfBirth
        {
            get
            {
                if (!BirthDay.HasValue && !BirthMonth.HasValue && !BirthYear.HasValue) return "";
                return $"{(BirthDay.HasValue ? BirthDay.Value.ToString() : "--")}/" +
                    $"{(BirthMonth.HasValue ? BirthMonth.Value.ToString() : "--")}/" +
                    $"{(BirthYear.HasValue ? BirthYear.Value.ToString() : "----")}";
            }
        }

        /// <summary>
        /// Tiền sử bệnh khác, nên hiển thị nếu là bệnh nhân (khách hàng)
        /// </summary>
        public string MedicalHistoryOther { get; set; }

        /// <summary>
        /// danh sach tieu su benh
        /// </summary>
        public IEnumerable<string> MedicalHistories { get; set; } = new List<string>();
    }
}
