using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using Infrastructure.Data;
using Microsoft.AspNetCore.StaticFiles;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class IrAttachmentRepository : EfRepository<IrAttachment>, IIrAttachmentRepository
    {
        public IrAttachmentRepository(IDbContext dbContext) : base(dbContext)
        {
        }

        public override async Task<IEnumerable<IrAttachment>> InsertAsync(IEnumerable<IrAttachment> entities)
        {
            foreach (var values in entities)
            {
                _CheckContents(values);
                _MakeThumbnail(values);
            }
            return await base.InsertAsync(entities);
        }

        private void _MakeThumbnail(IrAttachment values)
        {
            //tạo thumbnail nếu attachment là hình
            //if (values.DbDatas != null && string.IsNullOrEmpty(values.ResField))
            //{
            //    if (!string.IsNullOrEmpty(values.MineType) && Regex.IsMatch("image.*(gif|jpeg|jpg|png)", values.MineType))
            //    {
            //        var temp_image = ImageHelper.CropImage(Convert.ToBase64String(values.DbDatas), type: "center", width: 80, height: 80);
            //    }
            //}
        }

        private void _CheckContents(IrAttachment values)
        {
            if (string.IsNullOrEmpty(values.MineType))
                values.MineType = _ComputeMineType(values);
        }

        private string _ComputeMineType(IrAttachment values)
        {
            var provider = new FileExtensionContentTypeProvider();
            string mineType = null;
            if (!string.IsNullOrEmpty(values.MineType))
                mineType = values.MineType;
            if (mineType == null && !string.IsNullOrEmpty(values.DatasFname))
                provider.TryGetContentType(values.DatasFname, out mineType);
            if (mineType == null && !string.IsNullOrEmpty(values.Url))
                provider.TryGetContentType(values.Url, out mineType);
            //có thể viết hàm đoán minetype từ binary data, tạm thời viết sau
            return mineType ?? "application/octet-stream";
        }
    }
}
