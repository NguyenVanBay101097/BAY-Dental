﻿using ApplicationCore.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class AppointmentBasic
    {
        public Guid Id { get; set; }

        /// <summary>
        /// Mã phiếu hẹn
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Ngày hẹn
        /// </summary>
        public DateTime? Date { get; set; }

        /// <summary>
        /// Người hẹn
        /// </summary>
        public string UserId { get; set; }
        public ApplicationUserSimple User { get; set; }

        /// <summary>
        /// Bác sĩ
        /// </summary>
        public Guid? DoctorId { get; set; }
        public EmployeeSimple Doctor { get; set; }

        /// <summary>
        /// Trạng thái cuộc hẹn: xác nhận, khách đã tới hoặc đã hủy bỏ
        /// confirmed, done, cancel
        /// </summary>
        public string State { get; set; }

        //Hẹn khách hàng nào?
        public Guid PartnerId { get; set; }
        public PartnerSimple Partner { get; set; }
    }

    public class AppointmentDisplay
    {
        public AppointmentDisplay()
        {
            Date = DateTime.Now;
            State = "confirmed";
        }

        public Guid Id { get; set; }

        /// <summary>
        /// Mã phiếu hẹn
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Ngày hẹn
        /// </summary>
        public DateTime? Date { get; set; }

        /// <summary>
        /// Người hẹn
        /// </summary>
        public string UserId { get; set; }
        public ApplicationUserSimple User { get; set; }

        /// <summary>
        /// Trạng thái cuộc hẹn: xác nhận, khách đã tới hoặc đã hủy bỏ
        /// confirmed, done, cancel
        /// </summary>
        public string State { get; set; }

        //Hẹn khách hàng nào?
        public Guid PartnerId { get; set; }
        public PartnerSimple Partner { get; set; }

        public Guid CompanyId { get; set; }

        public Guid? DotKhamId { get; set; }

        /// <summary>
        /// Ghi chú, nội dung
        /// </summary>
        public string Note { get; set; }
        
        /// <summary>
        /// Bác sĩ
        /// </summary>
        public Guid? DoctorId { get; set; }
        public EmployeeSimple Doctor { get; set; }

        public bool HasDotKhamRef { get; set; }
    }
    public class AppointmentPaged
    {
        public AppointmentPaged()
        {
            Offset = 0;
        }

        public int Offset { get; set; }

        public int Limit { get; set; }

        public string Search { get; set; }

        //public string SearchByDoctor { get; set; }

        //public string SearchByAppointment { get; set; }

        public DateTime? DateTimeFrom { get; set; }

        public DateTime? DateTimeTo { get; set; }

        public string State { get; set; }

        public Guid? DotKhamId { get; set; }
    }

    public class AppointmentDefaultGet
    {
        public Guid? DotKhamId { get; set; }
    }

    public class AppointmentStateCount
    {
        public string State { get; set; }
        public string Color { get; set; }
        public int Count { get; set; }
    }

    public class DateFromTo
    {
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
    }

    public class AppointmentPatch
    {
        public Guid Id { get; set; }
        public string State { get; set; }
        public DateTime Date { get; set; }
    }
}
