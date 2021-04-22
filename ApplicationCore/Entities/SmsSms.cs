﻿using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    public class SmsSms : BaseEntity
    {
        public string Body { get; set; }

        public string Number { get; set; }

        public Guid? PartnerId { get; set; }
        public Partner Partner { get; set; }

        public string State { get; set; }

        public string ErrorCode { get; set; }
    }
}
