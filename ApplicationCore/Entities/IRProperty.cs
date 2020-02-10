using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ApplicationCore.Entities
{
    public class IRProperty : BaseEntity
    {
        public IRProperty()
        {
            Type = "many2one";
        }

        public string Name { get; set; }

        public double? ValueFloat { get; set; }

        public int? ValueInteger { get; set; }

        public string ValueText { get; set; }

        public byte[] ValueBinary { get; set; }

        public string ValueReference { get; set; }

        public DateTime? ValueDateTime { get; set; }

        public string Type { get; set; }

        public Guid? CompanyId { get; set; }
        public Company Company { get; set; }

        public Guid FieldId { get; set; }
        public IRModelField Field { get; set; }

        public string ResId { get; set; }

        [NotMapped]
        public object Value { get; set; }
    }
}
