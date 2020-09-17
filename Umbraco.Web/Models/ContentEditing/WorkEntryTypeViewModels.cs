using RestSharp.Extensions;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{

    public class WorkEntryTypeBasic
    {
        public Guid id { get; set; }
        public string Name { get; set; }
    }

    public class WorkEntryTypePaged
    {

        public WorkEntryTypePaged()
        {
            Limit = 20;
        }

        public int Limit { get; set; }

        public int Offset { get; set; }

        public string Filter { get; set; }
    }

    public class WorkEntryTypeSave
    {
        public string Name { get; set; }

        public bool IsHasTimeKeeping { get; set; } = false;

        public string Color { get; set; }

        public string Code { get; set; }

        public int? Sequence { get; set; }
        public string RoundDays { get; set; } = "NO";
        public string RoundDaysType { get; set; }

    }

    public class WorkEntryTypeDisplay
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public bool IsHasTimeKeeping { get; set; } = false;

        public string Color { get; set; }

        public string Code { get; set; }
        public string RoundDays { get; set; }
        public string RoundDaysType { get; set; }
        public int? Sequence { get; set; }
    }
}
