using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
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
                line.Name = string.Format("Ngày uống {0} lần, mỗi lần {1} viên, uống {2}", line.NumberOfTimes, line.AmountOfTimes,
                    GetUseAt(line.UseAt));
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
