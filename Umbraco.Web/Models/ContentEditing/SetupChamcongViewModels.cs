using RestSharp.Extensions;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{

    public class SetupChamcongDisplay
    {
        public decimal StandardWorkHour { get; set; }
        public decimal OneStandardWorkFrom { get; set; }
        public decimal OneStandardWorkTo { get; set; }
        public decimal HalfStandardWorkFrom { get; set; }
        public decimal HalfStandardWorkTo { get; set; }
    }
}
