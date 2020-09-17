using ApplicationCore.Entities;
using RestSharp.Extensions;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{

    public class ResourceCalendarLeaveBasic
    {
        public Guid Id { get; set; }

        public string Name { get; set; }
    }

    public class ResourceCalendarLeavePaged
    {

        public ResourceCalendarLeavePaged()
        {
            Limit = 20;
        }

        public int Limit { get; set; }

        public int Offset { get; set; }

        public string Filter { get; set; }
    }

    public class ResourceCalendarLeaveSave
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Guid? CompanyId { get; set; }

        //public Guid? CalendarId { get; set; }

        public DateTime DateFrom { get; set; }

        public DateTime DateTo { get; set; }

    }

    public class ResourceCalendarLeaveDisplay
    {
        public Guid ID { get; set; }
        public string Name { get; set; }
        public Guid? CompanyId { get; set; }

        public Guid? CalendarId { get; set; }

        public DateTime DateFrom { get; set; }

        public DateTime DateTo { get; set; }
    }
}
