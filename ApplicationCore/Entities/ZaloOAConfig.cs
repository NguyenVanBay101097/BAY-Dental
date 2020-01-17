using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    public class ZaloOAConfig: BaseEntity
    {
        public string Name { get; set; }
        public string AccessToken { get; set; }
        public string Avatar { get; set; }
    }
}
