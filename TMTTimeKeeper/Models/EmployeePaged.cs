using System;
using System.Collections.Generic;
using System.Text;

namespace TMTTimeKeeper.Models
{
    public class EmployeePaged
    {
        public int offset { get; set; }
        public int limit { get; set; }
        public string search { get; set; }
        public bool isDoctor { get; set; }
        public bool isAssistant { get; set; }
    }
}
