using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class SamplePrescriptionDisplay
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Note { get; set; }

        public IEnumerable<SamplePrescriptionLineDisplay> Lines = new List<SamplePrescriptionLineDisplay>();
    }
}
