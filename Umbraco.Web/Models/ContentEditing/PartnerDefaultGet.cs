using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class PartnerDefaultGet
    {
    }

    public class GenderPartner
    {
        public Guid? Id { get; set; }
        public string Name { get; set; }
    }

    public class GetThreadMessageForPartnerRequest
    {
        public GetThreadMessageForPartnerRequest()
        {
            Limit = 30;
        }

        public int Offset { get; set; }
        public int Limit { get; set; }

        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }

        public Guid? SubtypeId { get; set; }
    }

    public class GetPartnerThreadMessageResponse
    {
        public IEnumerable<MailMessageFormat> Messages { get; set; } = new List<MailMessageFormat>();
    }

    public class CreateCommentForPartnerRequest
    {
        public string body { get; set; }
    }
}
