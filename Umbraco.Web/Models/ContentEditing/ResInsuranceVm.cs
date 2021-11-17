﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class ResInsuranceBasic
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Phone { get; set; }

        public bool IsActive { get; set; }
    }

    public class ResInsuranceDisplay
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public DateTime Date { get; set; }

        public string Avatar { get; set; }

        /// <summary>
        /// người đại diện
        /// </summary>
        public string Representative { get; set; }

        public string Phone { get; set; }
        public string Email { get; set; }

        public string Address { get; set; }

        public string Note { get; set; }
    }

    public class ResInsuranceSave
    {
        public string Name { get; set; }

        public DateTime Date { get; set; }

        public string Avatar { get; set; }

        /// <summary>
        /// người đại diện
        /// </summary>
        public string Representative { get; set; }

        public string Phone { get; set; }
        public string Email { get; set; }

        public string Address { get; set; }

        public string Note { get; set; }
    }

    public class ResInsuranceSimple
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
    }
}
