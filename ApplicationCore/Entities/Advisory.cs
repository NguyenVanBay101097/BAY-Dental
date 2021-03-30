using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    public class Advisory : BaseEntity
    {
        /// <summary>
        /// Ngày tư vấn
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// người tư vấn
        /// </summary>
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

        public ICollection<AdvisoryToothRel> AdvisoryToothRels { get; set; } = new List<AdvisoryToothRel>();
        public ICollection<AdvisoryToothDiagnosisRel> AdvisoryToothDiagnosisRels { get; set; } = new List<AdvisoryToothDiagnosisRel>();
        public ICollection<AdvisoryProductRel> AdvisoryProductRels { get; set; } = new List<AdvisoryProductRel>();

    }
}
