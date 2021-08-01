using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TMTDentalAPI.Endpoints.BirthdayCustomerEndpoints
{
    public class BirthdayCustomerDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }

        public string Phone { get; set; }

        public decimal TotalQty { get; set; }

        public decimal PriceTotal { get; set; }

        public int? BirthDay { get; set; }

        public int? BirthMonth { get; set; }

        public int? BirthYear { get; set; }

        public string Gender { get; set; }

        public string DateOfBirth
        {
            get
            {
                if (!BirthDay.HasValue && !BirthMonth.HasValue && !BirthYear.HasValue) return "";
                return $"{(BirthDay.HasValue ? BirthDay.Value.ToString() : "--")}/" +
                    $"{(BirthMonth.HasValue ? BirthMonth.Value.ToString() : "--")}/" +
                    $"{(BirthYear.HasValue ? BirthYear.Value.ToString() : "----")}";
            }  
        }

        public string DisplayGender
        {
            get
            {
                switch (Gender)
                {
                    case "female":
                        return "Nữ";
                    case "other":
                        return "Khác";
                    default:
                        return "Nam";
                }
            }
        }
    }
}
