﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class ZaloOAConfigBasic
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Avatar { get; set; }
        public bool AutoSendBirthdayMessage { get; set; }
        public string BirthdayMessageContent { get; set; }
    }
}
