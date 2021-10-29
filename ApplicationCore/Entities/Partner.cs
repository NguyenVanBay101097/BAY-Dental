﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ApplicationCore.Entities
{
    /// <summary>
    /// Khách hàng
    /// </summary>
    public class Partner : BaseEntity
    {
        public Partner()
        {
            Customer = true;
            Active = true;
            Date = DateTime.Today;
            Gender = "male";
        }

        public string DisplayName { get; set; }

        public string Name { get; set; }

        public string NameNoSign { get; set; }

        /// <summary>
        /// Số nhà, đường
        /// </summary>
        public string Street { get; set; }

        public string Phone { get; set; }
        public string Email { get; set; }

        /// <summary>
        /// Check this box if this contact is a vendor. It can be selected in purchase orders.
        /// </summary>
        public bool Supplier { get; set; }

        /// <summary>
        /// Check this box if this contact is a customer. It can be selected in sales orders.
        /// </summary>
        public bool Customer { get; set; }

        public bool IsAgent { get; set; }

        /// <summary>
        /// Chi nhánh
        /// </summary>
        public Guid? CompanyId { get; set; }
        public Company Company { get; set; }

        /// <summary>
        /// Mã khách hàng
        /// </summary>
        public string Ref { get; set; }

        /// <summary>
        /// Ghi chú
        /// </summary>
        public string Comment { get; set; }

        /// <summary>
        /// Khả dụng
        /// </summary>
        public bool Active { get; set; }

        /// <summary>
        /// Check this box if this contact is an Employee.
        /// </summary>
        public bool Employee { get; set; }

        /// <summary>
        /// Giới tính
        /// ('male', 'Male')
        /// ('female', 'Female')
        /// ('other', 'Other')
        /// </summary>
        public string Gender { get; set; }

        /// <summary>
        /// Nghề nghiệp
        /// </summary>
        public string JobTitle { get; set; }

        /// <summary>
        /// Năm sinh
        /// </summary>
        public int? BirthYear { get; set; }

        /// <summary>
        /// Tháng sinh
        /// </summary>
        public int? BirthMonth { get; set; }

        /// <summary>
        /// Ngày sinh
        /// </summary>
        public int? BirthDay { get; set; }

        /// <summary>
        /// Tiền sử bệnh khác, nên hiển thị nếu là bệnh nhân (khách hàng)
        /// </summary>
        public string MedicalHistory { get; set; }

        public ICollection<PartnerHistoryRel> PartnerHistoryRels { get; set; } = new List<PartnerHistoryRel>();

        public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();

        public ICollection<CustomerReceipt> CustomerReceipts { get; set; } = new List<CustomerReceipt>();

        public ICollection<PartnerImage> PartnerImages { get; set; } = new List<PartnerImage>();

        /// <summary>
        /// Mã tỉnh/thành phố
        /// </summary>
        public string CityCode { get; set; }

        /// <summary>
        /// Tên tỉnh/thành phố
        /// </summary>
        public string CityName { get; set; }

        /// <summary>
        /// Mã quận/huyện
        /// </summary>
        public string DistrictCode { get; set; }

        /// <summary>
        /// Tên quận/huyện
        /// </summary>
        public string DistrictName { get; set; }

        /// <summary>
        /// Mã phường xã
        /// </summary>
        public string WardCode { get; set; }

        /// <summary>
        /// Tên phường xã
        /// </summary>
        public string WardName { get; set; }

        /// <summary>
        /// Mã vạch
        /// </summary>
        public string Barcode { get; set; }


        public ICollection<AccountMoveLine> AMoveLines { get; set; }

        public ICollection<PartnerPartnerCategoryRel> PartnerPartnerCategoryRels { get; set; } = new List<PartnerPartnerCategoryRel>();

        public string Fax { get; set; }

        /// <summary>
        /// Nguồn biết đến
        /// </summary>       
        public Guid? SourceId { get; set; }

        public PartnerSource Source { get; set; }

        /// <summary>
        /// Nhân viên giới thiệu
        /// </summary>
        public string ReferralUserId { get; set; }
        public ApplicationUser ReferralUser { get; set; }

        /// <summary>
        /// Ghi chú khi nguồn là 'Khác'
        /// </summary>
        public string Note { get; set; }

        /// <summary>
        /// Ảnh chân dung
        /// </summary>
        public string Avatar { get; set; }

        public string ZaloId { get; set; }

        /// <summary>
        /// Ngay lap ho so khach hang
        /// </summary>
        public DateTime? Date { get; set; }

        public Guid? TitleId { get; set; }
        public PartnerTitle Title { get; set; }

        ///// <summary>
        ///// Nhan vien tu van 
        ///// </summary>
        //public Guid? ConsultantId { get; set; }
        //public Employee Consultant { get; set; }

        public ICollection<SaleOrder> SaleOrders { get; set; } = new List<SaleOrder>();

        public ICollection<DotKham> DotKhams { get; set; } = new List<DotKham>();

        /// <summary>
        /// người giới thiệu
        /// </summary>
        public Guid? AgentId { get; set; }
        public Agent Agent { get; set; }

        /// <summary>
        /// khách hàng tạm ứng
        /// </summary>
        public ICollection<PartnerAdvance> PartnerAdvances { get; set; } = new List<PartnerAdvance>();

        /// <summary>
        /// Thẻ thành viên
        /// </summary>
        public ICollection<CardCard> CardCards { get; set; } = new List<CardCard>();

        public string GetAddress()
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

        [NotMapped]
        public string GetAge
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

        [NotMapped]
        public string GetGender
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
        }

        public string GetDateOfBirth()
        {
            if (!BirthDay.HasValue && !BirthMonth.HasValue && !BirthYear.HasValue) return "";
            return $"{(BirthDay.HasValue ? BirthDay.Value.ToString() : "--")}/" +
                $"{(BirthMonth.HasValue ? BirthMonth.Value.ToString() : "--")}/" +
                $"{(BirthYear.HasValue ? BirthYear.Value.ToString() : "----")}";
        }

        //public string GetGender()
        //{
        //    switch (Gender)
        //    {
        //        case "female":
        //            return "Nữ";
        //        case "other":
        //            return "Khác";
        //        default:
        //            return "Nam";
        //    }
        //}

        //public string GetAge()
        //{
        //    if (!BirthYear.HasValue)
        //    {
        //        return string.Empty;
        //    }

        //    return (DateTime.Now.Year - BirthYear.Value).ToString();
        //}

    }
}
