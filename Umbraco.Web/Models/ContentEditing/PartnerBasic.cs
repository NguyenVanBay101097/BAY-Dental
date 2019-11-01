﻿using System;
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

        public string Address { get; set; }

        public int? BirthYear { get; set; }
    }

    public class PartnerPaged
    {
        public PartnerPaged()
        {
            Limit = 20;
        }

        public int Offset { get; set; }

        public int Limit { get; set; }

        /// <summary>
        /// Search mã hoặc tên
        /// </summary>
        public string SearchNamePhoneRef { get; set; }

        public bool? Customer { get; set; }

        public bool? Employee { get; set; }

        public bool? Supplier { get; set; }
    }

    public class PartnerImportExcelViewModel
    {
        public string FileBase64 { get; set; }

        public bool Customer { get; set; }

        public bool Supplier { get; set; }

        public bool Employee { get; set; }
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

        public string Phone { get; set; }

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
}
