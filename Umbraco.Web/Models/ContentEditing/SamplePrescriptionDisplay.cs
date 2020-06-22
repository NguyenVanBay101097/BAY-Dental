using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class SamplePrescriptionDisplay
    {
        public Guid Id { get; set; }
        public string Name { get; set; }

        public IEnumerable<SamplePrescriptionLineSave> Lines = new List<SamplePrescriptionLineSave>();
    }
}
