using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using ApplicationCore.Models;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Umbraco.Web.Models.ContentEditing;

namespace Infrastructure.Services
{
    public class ProductStepService : BaseService<ProductStep>, IProductStepService
    {
        private readonly IMapper _mapper;
        public ProductStepService(IAsyncRepository<ProductStep> repository, IHttpContextAccessor httpContextAccessor,
            IMapper mapper) : base(repository, httpContextAccessor)
        {
            _mapper = mapper;
        }

        private IQueryable<ProductStep> GetQueryPaged()
        {
            var query = SearchQuery();

            query = query.OrderBy(s => s.Order);
            return query;
        }

        public IEnumerable<ProductStepDisplay> GetByProductId(Guid productId)
        {
            var query = GetQueryPaged();
            var productSteps = query.Where(x => x.ProductId == productId).ToList();
            return _mapper.Map<IEnumerable<ProductStepDisplay>>(productSteps);
        }

        //Sắp xếp lại thứ tự (Order) của các công đoạn
        public List<ProductStepDisplay> ReorderAsync(Guid? productId, int order)
        {
            var list = new List<ProductStepDisplay>();
            if (productId.HasValue)
            {
                list = GetByProductId(productId ?? Guid.Empty).Where(x => x.Order > order).OrderBy(x => x.Order).ToList();
                if (list.Count() > 0)
                {
                    var count = order;
                    for (var i = 0; i < list.Count(); i++)
                    {
                        list.ElementAt(i).Order = count;
                        count += 1;
                    }
                }                
            }
            return list;            
        }
    }
}
