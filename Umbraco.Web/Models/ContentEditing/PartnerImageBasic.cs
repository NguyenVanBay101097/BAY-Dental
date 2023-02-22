using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class PartnerImageBasic
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public Guid? PartnerId { get; set; }

        public Guid? DotKhamId { get; set; }

        public string UploadId { get; set; }

        public DateTime? Date { get; set; }

        public string Note { get; set; }
    }

    public class PartnerImageDisplay
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string UploadId { get; set; }

        public DateTime? Date { get; set; }
    }

    public class PartnerImageSave
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string UploadId { get; set; }
    }

    public class PartnerImageViewModel
    {
        public DateTime? Date { get; set; }
        public IEnumerable<PartnerImageBasic> PartnerImages { get; set; } = new List<PartnerImageBasic>();
    }
}
