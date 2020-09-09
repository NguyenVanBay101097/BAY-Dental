﻿using System;
using System.Collections.Generic;
using System.Text;

namespace TMTTimeKeeper.Models
{
    public class Employee
    {
        public Guid Id { get; set; }
        public int IdKP { get; set; }
        public string Name { get; set; }
        public int MachineNumber { get; set; }
        public string EnrollNumber { get; set; }
        public int FingerIndex { get; set; }
        public string TmpData { get; set; }
        public int Privelage { get; set; }
        public string Password { get; set; }
        public bool Enabled { get; set; }
        public string iFlag { get; set; }
    }
}
