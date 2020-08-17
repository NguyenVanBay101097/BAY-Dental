using ApplicationCore.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TMTDentalAPI.ViewModels;
using Umbraco.Web.Models.ContentEditing;

namespace Infrastructure.Services
{
    public interface IPartnerImageService : IBaseService<PartnerImage>
    {
        Task<IEnumerable<PartnerImageBasic>> SearchRead(PartnerImageSearchRead val);
        Task<IEnumerable<PartnerImageBasic>> BinaryUploadPartnerImage(UploadPartnerImageViewModel val);
    }
}
