using ApplicationCore.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Web.Models.ContentEditing;

namespace Infrastructure.Services
{
    public interface IProductStepService : IBaseService<ProductStep>
    {
        IEnumerable<ProductStepDisplay> GetByProductId(Guid productId);
        List<ProductStepDisplay> ReorderAsync(Guid? productId, int order);
    }
}
