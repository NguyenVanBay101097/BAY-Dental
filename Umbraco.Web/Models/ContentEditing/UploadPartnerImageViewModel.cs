using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TMTDentalAPI.ViewModels
{
    public class UploadPartnerImageViewModel
    {
        public DateTime? Date { get; set; }

        public string Note { get; set; }

        public Guid? PartnerId { get; set; }

        public Guid? DotkhamId { get; set; }

        public IList<IFormFile> files { get; set; }
    }
}
