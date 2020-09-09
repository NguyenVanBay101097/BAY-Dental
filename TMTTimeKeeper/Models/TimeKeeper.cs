using System;
using System.Collections.Generic;
using System.Text;

namespace TMTTimeKeeper.Models
{
    public class TimeKeeper
    {
        public int Id { get; set; }
        public string CompanyName { get; set; }
        public string Name {get;set;}
        public string Seri { get; set; }
        public string AddressIP { get; set; }
        public string ConnectTCP { get; set; }
    }
}
