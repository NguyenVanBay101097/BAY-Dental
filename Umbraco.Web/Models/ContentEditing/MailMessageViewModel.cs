using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class MailMessageSave
    {
        /// <summary>
        /// nội dung
        /// </summary>
        public string body { get; set; }


        public Guid? ResId { get; set; }

        public string Model { get; set; }

        public string MessageType { get; set; }

        /// <summary>
        /// many2one : các từ khóa định danh trong irmodeldata 
        /// </summary>
        public string Subtype { get; set; }

    }
}
