using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Web.Models.ContentEditing;

namespace Infrastructure.Services
{
    public class QuotationLineService : BaseService<QuotationLine>, IQuotationLineService
    {
        private readonly IMapper _mapper;

        public QuotationLineService(IMapper mapper, IAsyncRepository<QuotationLine> repository, IHttpContextAccessor httpContextAccessor) : base(repository, httpContextAccessor)
        {
            _mapper = mapper;
        }




        public void _ComputeAmountDiscountTotal(IEnumerable<QuotationLine> self)
        {
            //Trường hợp ưu đãi phiếu điều trị thì ko đúng, sum từ PromotionLines là đúng
            foreach (var line in self)
                line.AmountDiscountTotal = line.PromotionLines.Sum(x => x.PriceUnit);
        }

        public void ComputeAmount(IEnumerable<QuotationLine> self)
        {
            if (self == null)
                return;

            foreach (var line in self)
                line.Amount = Math.Round(line.Qty * ((line.SubPrice ?? 0) - (decimal)(line.AmountDiscountTotal ?? 0)));

        }

    }
}
