using MyERP.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ApplicationCore.Entities
{
    public abstract class AdminBaseEntity
    {
        public AdminBaseEntity()
        {
            Id = GuidComb.GenerateComb();
        }

        public Guid Id { get; set; }

        public string CreatedById { get; set; }
        public ApplicationAdminUser CreatedBy { get; set; }

        public string WriteById { get; set; }
        public ApplicationAdminUser WriteBy { get; set; }

        public DateTime? DateCreated { get; set; } = DateTime.Now;

        public DateTime? LastUpdated { get; set; } = DateTime.Now;
    }
}
