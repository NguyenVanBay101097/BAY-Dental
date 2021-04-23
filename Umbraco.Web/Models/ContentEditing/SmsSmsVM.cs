using ApplicationCore.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class SmsSmsBasic
    {
        public Guid Id { get; set; }
        public string Body { get; set; }
        public string Number { get; set; }
        public Partner Partner { get; set; }
        public string State { get; set; }
        public string ErrorCode { get; set; }

    }

    public class SmsSmsPaged
    {
        public SmsSmsPaged()
        {
            Limit = 20;
        }
        public int Limit { get; set; }
        public int Offset { get; set; }
        public string  Search { get; set; }
        public Guid? PartnerId { get; set; }
        public string State { get; set; }

    }
}
