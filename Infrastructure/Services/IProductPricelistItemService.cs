using ApplicationCore.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Services
{
    public interface IProductPricelistItemService: IBaseService<ProductPricelistItem>
    {
        void ValidateBase(IEnumerable<ProductPricelistItem> selfs);
    }
}
