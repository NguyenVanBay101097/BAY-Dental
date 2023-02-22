using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Umbraco.Web.Models.ContentEditing;

namespace TMTDentalAPI.Endpoints.PartnerEndpoints
{
    public class CreateSupplierRequest
    {
        public string Name { get; set; }

        public string Phone { get; set; }

        public string Street { get; set; }

        /// <summary>
        /// Notes
        /// </summary>
        public string Comment { get; set; }

        /// <summary>
        /// Mã tỉnh/thành phố
        /// </summary>
        public CitySimple City { get; set; }

        /// <summary>
        /// Tên quận/huyện
        /// </summary>
        public DistrictSimple District { get; set; }

        /// <summary>
        /// Tên phường xã
        /// </summary>
        public WardSimple Ward { get; set; }

        /// <summary>
        /// Email
        /// </summary>
        public string Email { get; set; }

        public DateTime? Date { get; set; }
    }
}
