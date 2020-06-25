using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class SamplePrescriptionSave
    {
        public string Name { get; set; }

        public IEnumerable<SamplePrescriptionLineSave> Lines { get; set; } = new List<SamplePrescriptionLineSave>();
    }
}
