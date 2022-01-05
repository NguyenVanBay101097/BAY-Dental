using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class LogForPartnerRequest
    {
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }

        public Guid? SubtypeId { get; set; }

        public string ThreadModel { get; set; }

        public Guid? ThreadId { get; set; }
    }

    public class TimeLineLogForPartnerResponse
    {
        public DateTime Date { get; set; }

        public List<LogForPartnerResponse> Logs { get; set; }
    }

    public class LogForPartnerResponse
    {
        public Guid Id { get; set; }

        public DateTime Date { get; set; }

        public string body { get; set; }

        public string SubtypeName { get; set; }

        public string UserName { get; set; }
    }

    public class MailMessageBasic
    {
        public Guid Id { get; set; }

        public DateTime Date { get; set; }

        public string body { get; set; }

        public string SubtypeName { get; set; }

        public string UserName { get; set; }
    }

    public class MailMessageSave
    {
        /// <summary>
        /// nội dung
        /// </summary>
        public string Body { get; set; }

        /// <summary>
        /// ResId
        /// </summary>
        public Guid? ThreadId { get; set; }

        /// <summary>
        /// model
        /// </summary>
        public string ThreadModel { get; set; }

        /// <summary>
        /// notification
        /// comment
        /// </summary>
        public string MessageType { get; set; }

        /// <summary>
        /// many2one : các từ khóa định danh trong irmodeldata 
        /// </summary>
        public string Subtype { get; set; }

    }
}
