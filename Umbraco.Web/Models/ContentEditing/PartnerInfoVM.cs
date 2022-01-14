﻿using ApplicationCore.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{

    public class PartnerInfoTemplate : PartnerInfoDisplay
    {
        public string MemberLevelId { get; set; }
        public string NameNoSign { get; set; }

        public DateTime? Date { get; set; }
        public string Gender { get; set; }
        public string CityName { get; set; }
        public string DistrictName { get; set; }
        public string WardName { get; set; }
        public string Street { get; set; }
        public string JobTitle { get; set; }
        public string Comment { get; set; }
        //doanh thu
        public decimal Revenue { get; set; }
        //nguồn
        public string SourceName { get; set; }

        public string CompanyName { get; set; }

        /// <summary>
        /// ngày hẹn gần nhất
        /// </summary>
        public DateTime? AppointmentDate { get; set; }

        /// <summary>
        /// ngày điều trị gần nhất
        /// </summary>
        public DateTime? SaleOrderDate { get; set; }

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
        public DateTime? DateCreated { get; set; }
        public string TitleName { get; set; }
        public bool Active { get; set; }
    }
    public class PartnerInfoDisplay
    {
        public Guid Id { get; set; }

        public string Ref { get; set; }

        public string Avatar { get; set; }

        public string DisplayName { get; set; }

        public string Name { get; set; }

        public string Phone { get; set; }

        public string Email { get; set; }

        public string CityName { get; set; }

        public string DistrictName { get; set; }

        public string WardName { get; set; }

        public int? BirthYear { get; set; }

        public int? BirthMonth { get; set; }

        public int? BirthDay { get; set; }

        public string OrderState { get; set; }

        public decimal? OrderResidual { get; set; }

        public decimal? TotalDebit { get; set; }

        public string JobTitle { get; set; }

        /// <summary>
        /// thẻ thành viên
        /// </summary>
        public string CardTypeName { get; set; }

        public string SourceName { get; set; }

        public string CompanyName { get; set; }

        /// <summary>
        /// ngày hẹn gần nhất
        /// </summary>
        public DateTime? AppointmentDate { get; set; }

        /// <summary>
        /// ngày điều trị gần nhất
        /// </summary>
        public DateTime? SaleOrderDate { get; set; }

        public MemberLevelBasic MemberLevel { get; set; }

        public IEnumerable<PartnerCategoryBasic> Categories { get; set; } = new List<PartnerCategoryBasic>();
        public string DateOfBirth
        {
            get
            {
                if (!BirthDay.HasValue && !BirthMonth.HasValue && !BirthYear.HasValue)
                    return string.Empty;

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

        public bool Active { get; set; }
    }

    public class PartnerInfoPaged
    {
        public PartnerInfoPaged()
        {
            Offset = 0;
            Limit = 20;
        }
        public int Limit { get; set; }
        public int Offset { get; set; }
        public string Search { get; set; }
        public IEnumerable<Guid> CategIds { get; set; } = new List<Guid>();
        /// <summary>
        /// 0: không có doanh thu, 1: có doanh thu
        /// </summary>
        public int? HasOrderResidual { get; set; }
        /// <summary>
        /// 0: không có , 1: có 
        /// </summary>
        public int? HasTotalDebit { get; set; }
        public Guid? MemberLevelId { get; set; }
        public string OrderState { get; set; }
        public Boolean? ShowInfo { get; set; }

        public Guid? CardTypeId { get; set; }

        public Guid? CompanyId { get; set; }
        public bool? Active { get; set; }
    }

    public class PartnerGetExistReq
    {
        public string Phone { get; set; }
        public bool? Customer { get; set; }
    }

    public class PartnerTreatmentHistoriesPrintRequest
    {
        public Guid Id { get; set; }
    }

    public class PartnerTreatmentHistoriesPrintResponse
    {
        public string DisplayName { get; set; }
        public IEnumerable<SaleOrderLineBasic> SaleOrderLines { get; set; } = new List<SaleOrderLineBasic>();
        public CompanyPrintVM Company { get; set; }
        public IEnumerable<HistorySimple> HistorySimples { get; set; } = new List<HistorySimple>();
    }

}
