using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    public class SmsMessageDetail : BaseEntity
    {
        public SmsMessageDetail()
        {
            State = "outgoing";
            Date = DateTime.Now;
        }

        public string Body { get; set; }
        public string Number { get; set; }
        public decimal Cost { get; set; }
        public DateTime? Date { get; set; }
        public Guid? PartnerId { get; set; }
        public Partner Partner { get; set; }
        public Guid SmsAccountId { get; set; }
        public SmsAccount SmsAccount { get; set; }

        //('outgoing', 'In Queue'),
        //('sent', 'Sent'),
        //('error', 'Error'),
        //('canceled', 'Canceled')
        public string State { get; set; }

        //('sms_number_missing', 'Missing Number'),
        //('sms_number_format', 'Wrong Number Format'),
        //('sms_credit', 'Insufficient Credit'),
        //('sms_server', 'Server Error'),
        //('sms_acc', 'Unregistered Account'),
        //('sms_blacklist', 'Blacklisted'),
        //('sms_duplicate', 'Duplicate'),
        //('sms_ip_server', 'IP Server'),
        //('sms_unknown', 'Unknown'),
        public string ErrorCode { get; set; }
        public Guid? SmsMessageId { get; set; }
        public SmsMessage SmsMessage { get; set; }
        public Guid? SmsCampaignId { get; set; }
        public SmsCampaign SmsCampaign { get; set; }
        public Guid? CompanyId { get; set; }
        public Company Company { get; set; }
    }
}
