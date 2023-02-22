using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    public class IRModelField : BaseEntity
    {
        public IRModelField()
        {
            TType = "many2one";
        }

        public string Model { get; set; }

        public Guid IRModelId { get; set; }
        public IRModel IRModel { get; set; }

        public string Name { get; set; }

        public string TType { get; set; }

        public string Relation { get; set; }
    }
}
