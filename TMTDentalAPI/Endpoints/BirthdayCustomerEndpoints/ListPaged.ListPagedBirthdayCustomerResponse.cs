using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TMTDentalAPI.Endpoints.BirthdayCustomerEndpoints
{
    public class ListPagedBirthdayCustomerResponse
    {
        public List<BirthdayCustomerDto> Items { get; set; } = new List<BirthdayCustomerDto>();
        public int TotalItems { get; set; }
    }
}
