using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    /// <summary>
    /// Nhóm nhân viên
    /// </summary>
    public class EmployeeCategory : BaseEntity
    {
        public EmployeeCategory()
        {
            Type = "other";
        }
        public string Name { get; set; }

        /// <summary>
        /// doctor: Bac si, assistant: phu ta, other: khac
        /// </summary>
        public string Type { get; set; }
    }
}
