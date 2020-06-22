﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class SamplePrescriptionBasic 
    {
        public Guid Id { get; set; }
        public string Name { get; set; }

        public IEnumerable<SamplePrescriptionLineBasic> Lines = new List<SamplePrescriptionLineBasic>();
    }
}
