using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class CreateCommentRequest
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

    }
}
