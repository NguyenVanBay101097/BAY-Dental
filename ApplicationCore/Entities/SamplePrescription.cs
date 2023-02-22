using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    public class SamplePrescription :BaseEntity
    {
        public string Name { get; set; }


        /// <summary>
        /// lời dặn
        /// </summary>
        public string Note { get; set; }

        public ICollection<SamplePrescriptionLine> Lines { get; set; } = new List<SamplePrescriptionLine>();
    }
}
