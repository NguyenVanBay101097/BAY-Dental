using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TMTDentalAPI.ViewModels
{
    public class UploadAttachmentViewModel
    {
        public string model { get; set; }

        public Guid? id { get; set; }

        [FromForm(Name = "files")]
        public IList<IFormFile> files { get; set; }
    }
}
