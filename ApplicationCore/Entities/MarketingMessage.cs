using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    public class MarketingMessage: BaseEntity
    {
        public MarketingMessage()
        {
            Type = "facebook";
            Template = "text";
        }

        public string Type { get; set; }

        //text: đơn giản,
        //button: mẫu nút
        public string Template { get; set; }

        public string Text { get; set; }

        public ICollection<MarketingMessageButton> Buttons { get; set; } = new List<MarketingMessageButton>();
    }
}
