﻿using MyERP.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ApplicationCore.Entities
{
    public abstract class BaseEntity
    {
        public BaseEntity()
        {
            var time = DateTime.Now;
            DateCreated = DateTime.Now;
            LastUpdated = DateTime.Now;
            //Id = GuidComb.GenerateComb();
        }

        public Guid Id { get; set; }

        public string CreatedById { get; set; }
        public ApplicationUser CreatedBy { get; set; }

        public string WriteById { get; set; }
        public ApplicationUser WriteBy { get; set; }

        public DateTime? DateCreated { get; set; }

        public DateTime? LastUpdated { get; set; }
    }
}
