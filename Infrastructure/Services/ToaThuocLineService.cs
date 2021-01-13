using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Umbraco.Web.Models.ContentEditing;

namespace Infrastructure.Services
{
    public class ToaThuocLineService : BaseService<ToaThuocLine>, IToaThuocLineService
    {

        public ToaThuocLineService(IAsyncRepository<ToaThuocLine> repository, IHttpContextAccessor httpContextAccessor)
        : base(repository, httpContextAccessor)
        {
        }

        public void ComputeName(IEnumerable<ToaThuocLine> self)
        {
            foreach (var line in self)
            {
                if (line.ProductUoM == null && line.ProductUoMId.HasValue)
                {
                    var uomObj = GetService<IUoMService>();
                    line.ProductUoM = uomObj.GetById(line.ProductUoMId);
                    line.Name = $"Ngày uống {line.NumberOfTimes} lần, mỗi lần {line.AmountOfTimes} {line.ProductUoM.Name}, uống {GetUseAt(line.UseAt)}";
                }  
                else if (line.Product == null)
                {
                    var productObj = GetService<IProductService>();
                    line.Product = productObj.SearchQuery(x => x.Id == line.ProductId).Include(x => x.UOM).FirstOrDefault();
                    line.Name = $"Ngày uống {line.NumberOfTimes} lần, mỗi lần {line.AmountOfTimes} {line.Product.UOM.Name}, uống {GetUseAt(line.UseAt)}";
                }
            }
        }

        private string GetUseAt(string useAt)
        {
            switch (useAt)
            {
                case "before_meal":
                    return "trước khi ăn";
                case "in_meal":
                    return "trong khi ăn";
                case "after_wakeup":
                    return "sau khi thức dậy";
                case "before_sleep":
                    return "trước khi đi ngủ";
                default:
                    return "sau khi ăn";
            }
        } 
    }
}
