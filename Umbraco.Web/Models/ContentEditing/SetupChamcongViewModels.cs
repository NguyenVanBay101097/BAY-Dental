using RestSharp.Extensions;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{

    public class SetupChamcongDisplay
    {
        public Guid? Id { get;set; }
        public decimal OneStandardWorkHour { get; set; }
        public decimal HalfStandardWorkHour { get; set; }
        public decimal DifferenceTime { get; set; }
    }
}
